using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Client.Services;

public interface ICategoriaService
{
    Task<List<Categoria>> GetCategoriasAsync();
    Task<Categoria> GetCategoriaByIdAsync(int id);
    Task<bool> AddCategoriaAsync(CategoriaAgregarDto categoria);
    Task<bool> UpdateCategoriaAsync(int id, CategoriaAgregarDto categoria);
    Task<bool> DeleteCategoriaAsync(int id);
}
