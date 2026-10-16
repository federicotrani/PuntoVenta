using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Server.Data;
using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;
using PuntoVenta.Shared.Enums;

namespace PuntoVenta.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VentasController : ControllerBase
{
    private const string UsuarioPorDefecto = "sistema";

    private readonly DataContext _context;
    private readonly ILogger<VentasController> _logger;

    public VentasController(DataContext context, ILogger<VentasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        try
        {
            var ventas = await _context.Ventas
                .AsNoTracking()
                .OrderByDescending(v => v.Fecha)
                .Select(v => new VentaListaDto
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    Estado = v.Estado,
                    FormaPago = v.FormaPago,
                    MontoTotal = v.MontoTotal
                })
                .ToListAsync();

            return Ok(ventas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ventas");
            return BadRequest("No se pudo obtener la lista de ventas.");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var venta = await _context.Ventas
                .AsNoTracking()
                .Include(v => v.VentaDetalle).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            return venta is null ? NotFound() : Ok(ADto(venta));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la venta {Id}", id);
            return BadRequest("No se pudo obtener la venta.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] VentaAgregarDto dto)
    {
        if (dto.Detalles.Count == 0)
        {
            return BadRequest("La venta debe tener al menos una línea.");
        }

        if (dto.Detalles.Any(d => d.Cantidad <= 0))
        {
            return BadRequest("Todas las cantidades deben ser mayores a cero.");
        }

        var lineas = dto.Detalles
            .GroupBy(d => d.ProductoId)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
            .ToList();

        await using var transaccion = await _context.Database.BeginTransactionAsync();

        try
        {
            var ids = lineas.Select(l => l.ProductoId).ToList();
            var productos = await _context.Productos
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var linea in lineas)
            {
                if (!productos.TryGetValue(linea.ProductoId, out var producto))
                {
                    return BadRequest($"El producto {linea.ProductoId} no existe.");
                }

                if (!producto.Activo)
                {
                    return BadRequest($"El producto '{producto.Nombre}' no está activo.");
                }

                if (producto.StockActual < linea.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.StockActual}.");
                }
            }

            var usuarioId = await ObtenerUsuarioPorDefectoAsync();
            var ahora = DateTime.Now;

            var venta = new Venta
            {
                Fecha = ahora,
                UsuarioId = usuarioId,
                Estado = EstadoVenta.Confirmada,
                FormaPago = dto.FormaPago,
                Notas = dto.Notas
            };

            foreach (var linea in lineas)
            {
                var producto = productos[linea.ProductoId];

                var filas = await _context.Productos
                    .Where(p => p.Id == linea.ProductoId && p.StockActual >= linea.Cantidad)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockActual, p => p.StockActual - linea.Cantidad));

                if (filas == 0)
                {
                    await transaccion.RollbackAsync();
                    return Conflict($"El stock de '{producto.Nombre}' cambió. Intente nuevamente.");
                }

                venta.VentaDetalle.Add(new VentaDetalle
                {
                    ProductoId = producto.Id,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            venta.MontoTotal = venta.VentaDetalle.Sum(d => d.MontoTotal);

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            foreach (var detalle in venta.VentaDetalle)
            {
                _context.Stocks.Add(new Stock
                {
                    Fecha = ahora,
                    TipoMovimiento = TipoMovimiento.Salida,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    UsuarioId = usuarioId,
                    Notas = $"Venta #{venta.Id}"
                });
            }

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            foreach (var detalle in venta.VentaDetalle)
            {
                detalle.Producto = productos[detalle.ProductoId];
            }

            return CreatedAtAction(nameof(ObtenerPorId), new { id = venta.Id }, ADto(venta));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar la venta");
            return BadRequest("No se pudo registrar la venta.");
        }
    }

    [HttpPost("{id:int}/anular")]
    public async Task<IActionResult> Anular(int id)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        try
        {
            var venta = await _context.Ventas
                .Include(v => v.VentaDetalle)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta is null)
            {
                return NotFound();
            }

            if (venta.Estado != EstadoVenta.Confirmada)
            {
                return BadRequest("Solo se pueden anular ventas confirmadas.");
            }

            var usuarioId = await ObtenerUsuarioPorDefectoAsync();
            var ahora = DateTime.Now;

            foreach (var detalle in venta.VentaDetalle)
            {
                var cantidad = detalle.Cantidad;

                await _context.Productos
                    .Where(p => p.Id == detalle.ProductoId)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockActual, p => p.StockActual + cantidad));

                _context.Stocks.Add(new Stock
                {
                    Fecha = ahora,
                    TipoMovimiento = TipoMovimiento.Entrada,
                    ProductoId = detalle.ProductoId,
                    Cantidad = cantidad,
                    UsuarioId = usuarioId,
                    Notas = $"Anulación de venta #{venta.Id}"
                });
            }

            venta.Estado = EstadoVenta.Anulada;
            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al anular la venta {Id}", id);
            return BadRequest("No se pudo anular la venta.");
        }
    }

    private async Task<int> ObtenerUsuarioPorDefectoAsync()
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == UsuarioPorDefecto);

        if (usuario is null)
        {
            usuario = new Usuario
            {
                NombreCompleto = "Usuario del sistema",
                NombreUsuario = UsuarioPorDefecto,
                Email = "sistema@puntoventa.local",
                HashPassword = "N/A",
                FechaAlta = DateTime.Now,
                Activo = true,
                Rol = RolUsuario.Cajero
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        return usuario.Id;
    }

    private static VentaDto ADto(Venta venta) => new()
    {
        Id = venta.Id,
        Fecha = venta.Fecha,
        Estado = venta.Estado,
        FormaPago = venta.FormaPago,
        MontoTotal = venta.MontoTotal,
        Notas = venta.Notas,
        Detalles = venta.VentaDetalle.Select(d => new VentaDetalleDto
        {
            ProductoId = d.ProductoId,
            ProductoNombre = d.Producto?.Nombre ?? string.Empty,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            Subtotal = d.MontoTotal
        }).ToList()
    };
}
