using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Shared.DTOs;

public record ProductoAgregarDto(string Nombre, decimal Precio, int CategoriaId, int StockActual=0, int StockMinimo = 0, string UrlImagen = "", bool Activo = true);

