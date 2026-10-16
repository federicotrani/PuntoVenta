using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Shared.DTOs;

public class LineaVentaDto
{
    public Producto Producto { get; set; } = null!;
    public int Cantidad { get; set; }
}
