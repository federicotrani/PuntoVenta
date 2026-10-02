namespace PuntoVenta.Shared;

public static class ImagenProductoReglas
{
    public const long TamanoMaximoBytes = 5 * 1024 * 1024;
    public const string CarpetaUpload = "upload";

    public static readonly string[] ExtensionesPermitidas = [".png", ".jpg", ".jpeg"];
    public static readonly string[] ContentTypesPermitidos = ["image/png", "image/jpeg"];
}
