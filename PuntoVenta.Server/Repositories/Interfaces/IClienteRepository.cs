using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Server.Repositories.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObtenerClientesAsync();
    Task<Cliente> ObtenerClientePorIdAsync(int id);
    Task<Cliente> CrearClienteAsync(Cliente cliente);
    Task<Cliente> ActualizarClienteAsync(Cliente cliente);
    Task<bool> EliminarClienteAsync(int id);
}
