using PuntoVenta.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Shared.DTOs;

public class VentaDetalleAgregarDto
{
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }
}

public class VentaAgregarDto
{
    public FormaPago FormaPago { get; set; }

    [MaxLength(1000)]
    public string? Notas { get; set; }

    [MinLength(1, ErrorMessage = "La venta debe tener al menos una línea.")]
    public List<VentaDetalleAgregarDto> Detalles { get; set; } = [];
}

public class VentaDetalleDto
{
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class VentaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoVenta Estado { get; set; }
    public FormaPago FormaPago { get; set; }
    public decimal MontoTotal { get; set; }
    public string? Notas { get; set; }
    public List<VentaDetalleDto> Detalles { get; set; } = [];
}

public class VentaListaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoVenta Estado { get; set; }
    public FormaPago FormaPago { get; set; }
    public decimal MontoTotal { get; set; }
}
