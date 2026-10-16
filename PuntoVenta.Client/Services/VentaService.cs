using PuntoVenta.Shared.DTOs;
using System.Net.Http.Json;

namespace PuntoVenta.Client.Services;

public interface IVentaService
{
    Task<List<VentaListaDto>> GetVentasAsync();
    Task<(VentaDto? Venta, string? Error)> RegistrarVentaAsync(VentaAgregarDto venta);
    Task<(bool Exito, string? Error)> AnularVentaAsync(int id);
}

public class VentaService : IVentaService
{
    private readonly ILogger<VentaService> _logger;
    private readonly HttpClient _http;

    public VentaService(ILogger<VentaService> logger, HttpClient http)
    {
        _logger = logger;
        _http = http;
    }

    public async Task<List<VentaListaDto>> GetVentasAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<VentaListaDto>>("api/ventas") ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ventas");
            return [];
        }
    }

    public async Task<(VentaDto? Venta, string? Error)> RegistrarVentaAsync(VentaAgregarDto venta)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync("api/ventas", venta);

            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<VentaDto>(), null);
            }

            return (null, await LeerErrorAsync(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar la venta");
            return (null, "No se pudo registrar la venta.");
        }
    }

    public async Task<(bool Exito, string? Error)> AnularVentaAsync(int id)
    {
        try
        {
            using var response = await _http.PostAsync($"api/ventas/{id}/anular", null);
            return response.IsSuccessStatusCode ? (true, null) : (false, await LeerErrorAsync(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al anular la venta {Id}", id);
            return (false, "No se pudo anular la venta.");
        }
    }

    private static async Task<string> LeerErrorAsync(HttpResponseMessage response)
    {
        var texto = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(texto) ? $"Error {(int)response.StatusCode}" : texto.Trim('"');
    }
}
