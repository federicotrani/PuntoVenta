using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Server.Data;
using PuntoVenta.Server.Services;
using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger _logger;
    private readonly ICategoriaPdfService _categoriaPdfService;

    public CategoriasController(DataContext context, ILogger<CategoriasController> logger, ICategoriaPdfService categoriaPdfService)
    {
        _logger = logger;
        _context = context;
        _categoriaPdfService = categoriaPdfService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        try
        {
            var categorias = await _context.Categorias.ToListAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            var msg = $"Error al obtener lista de categorias. Detalle: {ex.Message}";
            _logger.LogError(msg);
            return BadRequest(msg);
        }
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DescargarPdf()
    {
        try
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            var pdf = _categoriaPdfService.GenerarListado(categorias);
            return File(pdf, "application/pdf", "categorias.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar el PDF de categorías");
            return BadRequest("No se pudo generar el PDF de categorías.");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        try
        {
            var categoria = await _context.Categorias.FindAsync(Id);
            return Ok(categoria);
        }
        catch (Exception ex)
        {
            var msg = $"Error al obtener categoria con ID: {Id}. Detalle: {ex.Message}";
            _logger.LogError(msg);
            return BadRequest(msg);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(CategoriaAgregarDto categoriaDto)
    {
        try
        {
            var categoria = new Categoria()
            {
                Nombre = categoriaDto.Nombre,
                Descripcion = categoriaDto.Descripcion
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok();

        }catch(Exception ex)
        {
            var msg = $"Error al agregar categoria. Detalle: {ex.Message}";
            _logger.LogError(msg);
            return BadRequest(msg);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, CategoriaAgregarDto categoriaDto)
    {
        try
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria is null)
            {
                return NotFound();
            }

            categoria.Nombre = categoriaDto.Nombre;
            categoria.Descripcion = categoriaDto.Descripcion;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la categoría con ID {CategoriaId}", id);
            return BadRequest("No se pudo actualizar la categoría.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria is null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la categoría con ID {CategoriaId}", id);
            return BadRequest("No se pudo eliminar la categoría.");
        }
    }

}
