
using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Shared.Entities;

public class Cliente
{
    public int Id { get; set; }
    [MaxLength(150)]
    public string Nombre { get; set; }
    [MaxLength(150)]
    public string Domicilio { get; set; }
    [MaxLength(150)]
    public string Email { get; set; }
    public bool Activo { get; set; }

}
