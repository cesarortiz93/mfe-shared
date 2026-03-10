namespace MFE.Shared.Models;

// Modelo que representa al usuario autenticado
// Lo usan todos los MFEs para saber quién está logueado
public class UserSession
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();

    // Propiedad calculada: true si tiene un UserId válido
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
}
