using PuntoVenta.Shared;

namespace PuntoVenta.Server.Services;

public class ImagenInvalidaException : Exception
{
    public ImagenInvalidaException(string message) : base(message) { }
}

public interface IImagenProductoService
{
    Task<string> GuardarAsync(IFormFile archivo, CancellationToken cancellationToken = default);
    void Eliminar(string? nombreArchivo);
}

public class ImagenProductoService : IImagenProductoService
{
    private readonly string _carpeta;

    public ImagenProductoService(IWebHostEnvironment environment)
    {
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        _carpeta = Path.Combine(webRoot, ImagenProductoReglas.CarpetaUpload);
    }

    public async Task<string> GuardarAsync(IFormFile archivo, CancellationToken cancellationToken = default)
    {
        if (archivo.Length == 0)
        {
            throw new ImagenInvalidaException("El archivo de imagen está vacío.");
        }

        if (archivo.Length > ImagenProductoReglas.TamanoMaximoBytes)
        {
            throw new ImagenInvalidaException("La imagen no puede superar los 5 MB.");
        }

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        if (!ImagenProductoReglas.ExtensionesPermitidas.Contains(extension)
            || !ImagenProductoReglas.ContentTypesPermitidos.Contains(archivo.ContentType.ToLowerInvariant()))
        {
            throw new ImagenInvalidaException("Solo se permiten imágenes PNG o JPG.");
        }

        await using var entrada = archivo.OpenReadStream();
        var cabecera = new byte[8];
        var leidos = await entrada.ReadAsync(cabecera, cancellationToken);

        if (!TieneFirmaValida(extension, cabecera.AsSpan(0, leidos)))
        {
            throw new ImagenInvalidaException("El contenido del archivo no corresponde a una imagen PNG o JPG válida.");
        }

        entrada.Position = 0;
        Directory.CreateDirectory(_carpeta);

        var nombreArchivo = $"{Guid.NewGuid():N}{extension}";
        var rutaCompleta = Path.Combine(_carpeta, nombreArchivo);

        await using var salida = new FileStream(rutaCompleta, FileMode.CreateNew, FileAccess.Write);
        await entrada.CopyToAsync(salida, cancellationToken);

        return nombreArchivo;
    }

    public void Eliminar(string? nombreArchivo)
    {
        if (string.IsNullOrWhiteSpace(nombreArchivo))
        {
            return;
        }

        var rutaCompleta = Path.Combine(_carpeta, Path.GetFileName(nombreArchivo));

        if (File.Exists(rutaCompleta))
        {
            File.Delete(rutaCompleta);
        }
    }

    private static bool TieneFirmaValida(string extension, ReadOnlySpan<byte> cabecera)
    {
        if (extension == ".png")
        {
            return cabecera.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        }

        return cabecera.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF });
    }
}
