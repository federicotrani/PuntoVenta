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

    public async Task<bool> AddCategoriaAsync(CategoriaAgregarDto categoria)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync("api/categorias", categoria);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar la categoría {Nombre}", categoria.Nombre);
            return false;
        }
    }

    public async Task<bool> DeleteCategoriaAsync(int id)
    {
        try
        {
            using var response = await _http.DeleteAsync($"api/categorias/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la categoría con id {CategoriaId}", id);
            return false;
        }
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

    public async Task<byte[]?> GetCategoriasPdfAsync()
    {
        try
        {
            using var response = await _http.GetAsync("api/categorias/pdf");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar el PDF de categorías");
            return null;
        }
    }

    public async Task UpdateCategoriaAsync(int id, CategoriaAgregarDto categoria)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/Categorias/{id}",
            categoria);

        response.EnsureSuccessStatusCode();
    }
}
