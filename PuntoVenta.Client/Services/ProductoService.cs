using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;
using System.Net.Http.Json;

namespace PuntoVenta.Client.Services;

public class ProductoService : IProductoService
{
    private readonly ILogger<ProductoService> _logger;
    private readonly HttpClient _http;

    public ProductoService(ILogger<ProductoService> logger, HttpClient http)
    {
        _logger = logger;
        _http = http;
    }

    public async Task<bool> AddProductoAsync(ProductoAgregarDto producto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/productos", producto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar el producto");
            return false;
        }
    }

    public async Task<bool> DeleteProductoAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/productos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el producto con id {ProductoId}", id);
            return false;
        }
    }

    public async Task<Producto?> GetProductoByIdAsync(int id)
    {
        try
        {
            using var response = await _http.GetAsync($"api/productos/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Producto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto con id {ProductoId}", id);
            return null;
        }
    }

    public async Task<bool> UpdateProductoAsync(int id, ProductoAgregarDto producto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/productos/{id}", producto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el producto con id {ProductoId}", id);
            return false;
        }
    }

    public async Task<List<Producto>> GetProductosAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Producto>>("api/productos") ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de productos");
            return [];
        }
    }

}
