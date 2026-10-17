using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuntoVenta.Server.Repositories.Interfaces;
using PuntoVenta.Shared.Entities;
using PuntoVenta.Shared.Helpers;

namespace PuntoVenta.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IClienteRepository clienteRepository, ILogger<ClientesController> logger)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerClientes()
    {
        try
        {
            var clientes = await _clienteRepository.ObtenerClientesAsync();
            return Ok(clientes);
        }catch(Exception ex)
        {
            // Log the exception or handle it as needed
            _logger.LogError($"Error al obtener clientes: {ex.Message}");
            return StatusCode(500, $"Error al obtener clientes: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerClientePorId(int id)
    {
        try
        {
            var cliente = await _clienteRepository.ObtenerClientePorIdAsync(id);

            // uso de helpers para encriptar el nombre del cliente antes de devolverlo
            // var datoEncriptado = Encriptacion.Encriptar(cliente.Nombre);

            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            _logger.LogError($"Error al obtener cliente por ID {id}: {ex.Message}");
            return StatusCode(500, $"Error al obtener cliente por ID {id}: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CrearCliente([FromBody] Cliente cliente)
    {
        try
        {
            var nuevoCliente = await _clienteRepository.CrearClienteAsync(cliente);
            return CreatedAtAction(nameof(ObtenerClientePorId), new { id = nuevoCliente.Id }, nuevoCliente);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return StatusCode(500, $"Error al crear cliente: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarCliente(int id, [FromBody] Cliente cliente)
    {
        try
        {
            if (id != cliente.Id)
            {
                return BadRequest("El ID del cliente no coincide.");
            }

            var clienteActualizado = await _clienteRepository.ActualizarClienteAsync(cliente);
            return Ok(clienteActualizado);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return StatusCode(500, $"Error al actualizar cliente: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCliente(int id)
    {
        try
        {
            var eliminado = await _clienteRepository.EliminarClienteAsync(id);
            if (eliminado)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return StatusCode(500, $"Error al eliminar cliente: {ex.Message}");
        }
    }
}
