using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Server.Data;
using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(DataContext context, ILogger<ProductosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        try
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .ToListAsync();

            return Ok(productos);
        }
        catch (Exception ex)
        {
            var mensaje = $"Error al obtener la lista de productos. Detalle: {ex.Message}";
            _logger.LogError(ex, mensaje);
            return BadRequest(mensaje);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto is null ? NotFound() : Ok(producto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto con id {ProductoId}", id);
            return BadRequest("No se pudo obtener el producto.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(ProductoAgregarDto productoDto)
    {
        try
        {
            if (!await _context.Categorias.AnyAsync(c => c.Id == productoDto.CategoriaId))
            {
                return BadRequest("La categoría indicada no existe.");
            }

            var producto = CrearProducto(productoDto);
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar el producto");
            return BadRequest("No se pudo agregar el producto.");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ProductoAgregarDto productoDto)
    {
        try
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto is null)
            {
                return NotFound();
            }

            if (!await _context.Categorias.AnyAsync(c => c.Id == productoDto.CategoriaId))
            {
                return BadRequest("La categoría indicada no existe.");
            }

            producto.Nombre = productoDto.Nombre;
            producto.Precio = productoDto.Precio;
            producto.CategoriaId = productoDto.CategoriaId;
            producto.StockActual = productoDto.StockActual;
            producto.StockMinimo = productoDto.StockMinimo;
            producto.UrlImagen = productoDto.UrlImagen;
            producto.Activo = productoDto.Activo;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el producto con id {ProductoId}", id);
            return BadRequest("No se pudo actualizar el producto.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto is null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el producto con id {ProductoId}", id);
            return BadRequest("No se pudo eliminar el producto.");
        }
    }

    private static Producto CrearProducto(ProductoAgregarDto productoDto)
    {
        return new Producto
        {
            Nombre = productoDto.Nombre,
            Precio = productoDto.Precio,
            CategoriaId = productoDto.CategoriaId,
            StockActual = productoDto.StockActual,
            StockMinimo = productoDto.StockMinimo,
            UrlImagen = productoDto.UrlImagen,
            Activo = productoDto.Activo
        };
    }
}
