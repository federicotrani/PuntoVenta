using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;
using System.Net.Http.Json;


namespace PuntoVenta.Client.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ILogger<CategoriaService> _logger;
    private readonly HttpClient _http;

    public CategoriaService(ILogger<CategoriaService> logger, HttpClient http)
    {
        _logger = logger;
        _http = http;
    }

    public Task<bool> AddCategoriaAsync(CategoriaAgregarDto categoria)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCategoriaAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Categoria> GetCategoriaByIdAsync(int id)
    {
        try
        {
            var categoria = await _http.GetFromJsonAsync<Categoria>($"api/categorias/{id}");
            return categoria;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener la categoria con id {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Categoria>> GetCategoriasAsync()
    {
        try
        {
            var categorias = await _http.GetFromJsonAsync<List<Categoria>>("api/categorias");
            return categorias;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener las categorias: {ex.Message}");
            return null;
        }
    }

    public Task<bool> UpdateCategoriaAsync(int id, CategoriaAgregarDto categoria)
    {
        throw new NotImplementedException();
    }
}
