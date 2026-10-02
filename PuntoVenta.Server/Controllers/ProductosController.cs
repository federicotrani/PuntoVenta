using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Server.Data;
using PuntoVenta.Server.Services;
using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private const int LimiteSolicitudBytes = 6 * 1024 * 1024;

    private readonly DataContext _context;
    private readonly ILogger<ProductosController> _logger;
    private readonly IImagenProductoService _imagenes;

    public ProductosController(DataContext context, ILogger<ProductosController> logger, IImagenProductoService imagenes)
    {
        _context = context;
        _logger = logger;
        _imagenes = imagenes;
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
    [RequestSizeLimit(LimiteSolicitudBytes)]
    public async Task<IActionResult> Agregar([FromForm] string producto, IFormFile? imagen)
    {
        string? nuevaImagen = null;

        try
        {
            var productoDto = JsonSerializer.Deserialize<ProductoAgregarDto>(producto, JsonSerializerOptions.Web);

            if (productoDto is null)
            {
                return BadRequest("Los datos del producto no son válidos.");
            }

            if (!await _context.Categorias.AnyAsync(c => c.Id == productoDto.CategoriaId))
            {
                return BadRequest("La categoría indicada no existe.");
            }

            var entidad = CrearProducto(productoDto);

            if (imagen is not null)
            {
                nuevaImagen = await _imagenes.GuardarAsync(imagen);
                entidad.UrlImagen = nuevaImagen;
            }

            _context.Productos.Add(entidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.Id }, entidad);
        }
        catch (ImagenInvalidaException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _imagenes.Eliminar(nuevaImagen);
            _logger.LogError(ex, "Error al agregar el producto");
            return BadRequest("No se pudo agregar el producto.");
        }
    }

    [HttpPut("{id:int}")]
    [RequestSizeLimit(LimiteSolicitudBytes)]
    public async Task<IActionResult> Actualizar(int id, [FromForm] string producto, IFormFile? imagen)
    {
        string? nuevaImagen = null;

        try
        {
            var productoDto = JsonSerializer.Deserialize<ProductoAgregarDto>(producto, JsonSerializerOptions.Web);

            if (productoDto is null)
            {
                return BadRequest("Los datos del producto no son válidos.");
            }

            var entidad = await _context.Productos.FindAsync(id);

            if (entidad is null)
            {
                return NotFound();
            }

            if (!await _context.Categorias.AnyAsync(c => c.Id == productoDto.CategoriaId))
            {
                return BadRequest("La categoría indicada no existe.");
            }

            entidad.Nombre = productoDto.Nombre;
            entidad.Precio = productoDto.Precio;
            entidad.CategoriaId = productoDto.CategoriaId;
            entidad.StockActual = productoDto.StockActual;
            entidad.StockMinimo = productoDto.StockMinimo;
            entidad.Activo = productoDto.Activo;

            var imagenAnterior = entidad.UrlImagen;

            if (imagen is not null)
            {
                nuevaImagen = await _imagenes.GuardarAsync(imagen);
                entidad.UrlImagen = nuevaImagen;
            }

            await _context.SaveChangesAsync();

            if (nuevaImagen is not null)
            {
                _imagenes.Eliminar(imagenAnterior);
            }

            return NoContent();
        }
        catch (ImagenInvalidaException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _imagenes.Eliminar(nuevaImagen);
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

            producto.Activo = false;
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
            Activo = productoDto.Activo
        };
    }
}
