using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

    public async Task<bool> AddProductoAsync(ProductoAgregarDto producto, ImagenUploadDto? imagen = null)
    {
        try
        {
            using var contenido = CrearContenido(producto, imagen);
            var response = await _http.PostAsync("api/productos", contenido);
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

    public async Task<bool> UpdateProductoAsync(int id, ProductoAgregarDto producto, ImagenUploadDto? imagen = null)
    {
        try
        {
            using var contenido = CrearContenido(producto, imagen);
            var response = await _http.PutAsync($"api/productos/{id}", contenido);
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

    private static MultipartFormDataContent CrearContenido(ProductoAgregarDto producto, ImagenUploadDto? imagen)
    {
        var contenido = new MultipartFormDataContent
        {
            { new StringContent(JsonSerializer.Serialize(producto, JsonSerializerOptions.Web), Encoding.UTF8), "producto" }
        };

        if (imagen is not null)
        {
            var archivo = new ByteArrayContent(imagen.Contenido);
            archivo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType);
            contenido.Add(archivo, "imagen", imagen.NombreArchivo);
        }

        return contenido;
    }

}
