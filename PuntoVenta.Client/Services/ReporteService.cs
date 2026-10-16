namespace PuntoVenta.Client.Services;

public interface IReporteService
{
    Task<byte[]?> GetCategoriasPdfAsync();
    Task<byte[]?> GetProductosPdfAsync();
    Task<byte[]?> GetVentasPdfAsync();
}

public class ReporteService : IReporteService
{
    private readonly ILogger<ReporteService> _logger;
    private readonly HttpClient _http;

    public ReporteService(ILogger<ReporteService> logger, HttpClient http)
    {
        _logger = logger;
        _http = http;
    }

    public Task<byte[]?> GetCategoriasPdfAsync() => DescargarAsync("api/reportes/categorias");

    public Task<byte[]?> GetProductosPdfAsync() => DescargarAsync("api/reportes/productos");

    public Task<byte[]?> GetVentasPdfAsync() => DescargarAsync("api/reportes/ventas");

    private async Task<byte[]?> DescargarAsync(string url)    {
        try
        {
            using var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar el reporte {Url}", url);
            return null;
        }
    }
}
