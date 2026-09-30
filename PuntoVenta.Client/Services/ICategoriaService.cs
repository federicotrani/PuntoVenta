using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Client.Services;

public interface ICategoriaService
{
    Task<List<Categoria>> GetCategoriasAsync();
    Task<byte[]?> GetCategoriasPdfAsync();
    Task<Categoria> GetCategoriaByIdAsync(int id);
    Task<bool> AddCategoriaAsync(CategoriaAgregarDto categoria);
    Task UpdateCategoriaAsync(int id, CategoriaAgregarDto categoria);
    Task<bool> DeleteCategoriaAsync(int id);
}
