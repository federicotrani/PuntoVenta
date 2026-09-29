using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Shared.Entities;

public class VentaDetalle
{
    public int Id { get; set; }
    [ForeignKey("Venta")]
    public int VentaId { get; set; }
    [ForeignKey("Producto")]
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; } = null!;
    public int Cantidad { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioUnitario { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoTotal => (Cantidad * PrecioUnitario);
}
