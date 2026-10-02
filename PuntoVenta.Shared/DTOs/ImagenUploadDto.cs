namespace PuntoVenta.Shared.DTOs;

public record ImagenUploadDto(string NombreArchivo, string ContentType, byte[] Contenido);
