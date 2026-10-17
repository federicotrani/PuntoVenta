using PuntoVenta.Server.Data;
using PuntoVenta.Server.Repositories.Interfaces;
using PuntoVenta.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace PuntoVenta.Server.Repositories;

public class ClienteRepository : IClienteRepository
{

    private readonly DataContext _dataContext;

    public ClienteRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Cliente> ActualizarClienteAsync(Cliente cliente)
    {
        try
        {
            _dataContext.Clientes.Update(cliente);
            await _dataContext.SaveChangesAsync();
            return await Task.FromResult(cliente);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("Error al actualizar el cliente.", ex);
        }
    }

    public async Task<Cliente> CrearClienteAsync(Cliente cliente)
    {
        try
        {
            _dataContext.Clientes.Add(cliente);
            _dataContext.SaveChanges();
            return await Task.FromResult(cliente);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("Error al crear el cliente.", ex);
        }
    }

    public Task<bool> EliminarClienteAsync(int id)
    {
        try
        {
            var cliente = _dataContext.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente != null)
            {
                _dataContext.Clientes.Remove(cliente);
                _dataContext.SaveChanges();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }

        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception($"Error al eliminar el cliente con ID {id}.", ex);
        }
    }

    public async Task<Cliente> ObtenerClientePorIdAsync(int id)
    {
        try
        {
            var cliente = await _dataContext.Clientes.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
            return cliente!;

        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception($"Error al obtener el cliente con ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<Cliente>> ObtenerClientesAsync()
    {
        try
        {
            var clientes = await _dataContext.Clientes.Where(c => c.Activo).ToListAsync();
            return clientes;

        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("Error al obtener los clientes.", ex);
        }
    }
}
