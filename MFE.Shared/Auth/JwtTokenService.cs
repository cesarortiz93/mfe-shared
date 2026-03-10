using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MFE.Shared.Auth;

public class JwtTokenService
{
    private readonly JwtConfig _config;

    public JwtTokenService(JwtConfig config)
    {
        _config = config;
    }

    // Genera un nuevo token JWT — solo lo usa el Shell
    public string GenerateToken(string userId, string email,
                                IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config.SecretKey));
        var creds = new SigningCredentials(key,
                        SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        // Agrega cada rol como un claim separado
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Valida un token y devuelve el usuario — lo usan todos los MFEs
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_config.SecretKey));

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _config.Issuer,
            ValidateAudience = true,
            ValidAudience = _config.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token,
                                validationParams, out _);
            return principal;
        }
        catch
        {
            return null; // Token inválido o expirado
        }
    }
}
