using PuntoVenta.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PuntoVenta.Shared.Entities;

public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoTotal { get; set; }
    [ForeignKey("Usuario")]
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public EstadoVenta Estado { get; set; }
    public FormaPago FormaPago { get; set; }
    [MaxLength(1000)]
    public string? Notas { get; set; }
    [JsonIgnore]
    public ICollection<VentaDetalle> VentaDetalle { get; set; } = [];
}
