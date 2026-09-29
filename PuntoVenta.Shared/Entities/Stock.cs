using PuntoVenta.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Shared.Entities;

public class Stock
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }
    [ForeignKey("Producto")]
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    [ForeignKey("Usuario")]
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    [MaxLength(1000)]
    public string? Notas { get; set; }
}
