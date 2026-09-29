using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Shared.Entities;

public class Producto
{
    public int Id { get; set; }
    [MaxLength(200)]
    public string Nombre { get; set; } = null!;
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Precio { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    [MaxLength(300)]
    public string? UrlImagen { get; set; } 
    public bool Activo { get; set; }
    [ForeignKey("Categoria")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
