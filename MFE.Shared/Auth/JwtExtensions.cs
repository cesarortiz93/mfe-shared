using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MFE.Shared.Auth;

// Método de extensión: permite escribir
//   builder.Services.AddMfeJwtAuthentication(config)
// en lugar de repetir toda la configuración en cada proyecto
public static class JwtExtensions
{
    public static IServiceCollection AddMfeJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfig = configuration
            .GetSection(JwtConfig.Section)
            .Get<JwtConfig>()
            ?? throw new InvalidOperationException(
                "Falta la sección [Jwt] en appsettings.json");

        services.AddSingleton(jwtConfig);
        services.AddSingleton<JwtTokenService>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtConfig.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                // Lee el JWT desde la cookie (no del header)
                // Esto es importante para Blazor Server
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request
                            .Cookies["mfe_auth_token"];
                        if (!string.IsNullOrEmpty(token))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
