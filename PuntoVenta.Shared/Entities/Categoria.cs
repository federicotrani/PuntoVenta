using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Shared.Entities;

public class Categoria
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;
    [MaxLength(300)]
    public string Descripcion { get; set; } = string.Empty;
}
