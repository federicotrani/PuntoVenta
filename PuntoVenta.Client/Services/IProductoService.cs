using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Client.Services;

public interface IProductoService
{
    Task<List<Producto>> GetProductosAsync();
    Task<Producto?> GetProductoByIdAsync(int id);
    Task<bool> AddProductoAsync(ProductoAgregarDto producto);
    Task<bool> UpdateProductoAsync(int id, ProductoAgregarDto producto);
    Task<bool> DeleteProductoAsync(int id);
}
