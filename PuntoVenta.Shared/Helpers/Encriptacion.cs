namespace PuntoVenta.Shared.Helpers;

public static class Encriptacion
{
    public static string Encriptar(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        byte[] textoBytes = System.Text.Encoding.UTF8.GetBytes(texto);
        return Convert.ToBase64String(textoBytes);
    }

    public static string Desencriptar(string textoEncriptado)
    {
        if (string.IsNullOrEmpty(textoEncriptado))
            return string.Empty;
        byte[] textoBytes = Convert.FromBase64String(textoEncriptado);
        return System.Text.Encoding.UTF8.GetString(textoBytes);
    }
}
