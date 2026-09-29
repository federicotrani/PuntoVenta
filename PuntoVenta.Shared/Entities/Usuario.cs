using PuntoVenta.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Shared.Entities;

public class Usuario
{
    public int Id { get; set; }
    [MaxLength(200)]
    public string NombreCompleto { get; set; } = null!;
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = null!;
    [MaxLength(200)]
    public string Email { get; set; } = null!;
    [MaxLength(100)]
    public string HashPassword { get; set; } = null!;
    public DateTime FechaAlta { get; set; }
    public bool Activo { get; set; }
    public RolUsuario Rol { get; set; }
}
