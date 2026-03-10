namespace MFE.Shared.Auth;

// Esta clase mapea la configuración del appsettings.json
// o las variables de entorno al objeto que usa el código
public class JwtConfig
{
    // Nombre de la sección en appsettings.json
    public const string Section = "Jwt";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}
