using PuntoVenta.Shared.DTOs;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Client.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ILogger<CategoriaService> _logger;
    private readonly DataContext _context;

    public CategoriaService(ILogger<CategoriaService> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<bool> AddCategoriaAsync(CategoriaAgregarDto categoria)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCategoriaAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Categoria> GetCategoriaAsync(int id)
    {
        try
        {
            await _context.
        }catch(Exception ex)
        {
            _logger.LogError($"");
        }
    }

    public Task<List<Categoria>> GetCategoriasAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateCategoriaAsync(int id, CategoriaAgregarDto categoria)
    {
        throw new NotImplementedException();
    }
}
