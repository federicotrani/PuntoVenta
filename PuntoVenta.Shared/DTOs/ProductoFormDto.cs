using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Shared.DTOs;

public class ProductoFormDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(0.01, 999999999.99, ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal Precio { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    public int CategoriaId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int StockActual { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }

    [StringLength(300, ErrorMessage = "La URL no puede superar los 300 caracteres.")]
    public string UrlImagen { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ProductoAgregarDto ToAgregarDto() =>
        new(Nombre.Trim(), Precio, CategoriaId, StockActual, StockMinimo, UrlImagen.Trim(), Activo);
}
