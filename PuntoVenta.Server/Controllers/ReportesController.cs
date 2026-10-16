using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Server.Data;
using PuntoVenta.Server.Services;

namespace PuntoVenta.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<ReportesController> _logger;
        private readonly IReportePdfService _reportePdfService;

        public ReportesController(DataContext context, ILogger<ReportesController> logger, IReportePdfService reportePdfService)
        {
            _context = context;
            _logger = logger;
            _reportePdfService = reportePdfService;
        }

        [HttpGet("categorias")]
        public async Task<IActionResult> Categorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .AsNoTracking()
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();

                var pdf = _reportePdfService.GenerarListadoCategorias(categorias);
                return File(pdf, "application/pdf", "categorias.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el PDF de categorías");
                return BadRequest("No se pudo generar el PDF de categorías.");
            }
        }

        [HttpGet("ventas")]
        public async Task<IActionResult> Ventas()
        {
            try
            {
                var ventas = await _context.Ventas
                    .AsNoTracking()
                    .OrderByDescending(v => v.Fecha)
                    .ToListAsync();

                var pdf = _reportePdfService.GenerarListadoVentas(ventas);
                return File(pdf, "application/pdf", "ventas.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el PDF de ventas");
                return BadRequest("No se pudo generar el PDF de ventas.");
            }
        }

        [HttpGet("productos")]
        public async Task<IActionResult> Productos()
        {
            try
            {
                var productos = await _context.Productos
                    .AsNoTracking()
                    .Include(p => p.Categoria)
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                var pdf = _reportePdfService.GenerarListadoProductos(productos);
                return File(pdf, "application/pdf", "productos.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el PDF de productos");
                return BadRequest("No se pudo generar el PDF de productos.");
            }
        }
    }
}
