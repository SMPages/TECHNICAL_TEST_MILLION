using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RealEstate.Api.Security;
/// <summary>
/// Configura autenticación JWT Bearer para la API.
/// </summary>
public static class JwtSetup
{

    /// <summary>
    /// Registra autenticación JWT Bearer usando configuración de <paramref name="config"/>.
    /// </summary>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var key = config["Jwt:Key"];
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        if (!string.IsNullOrWhiteSpace(key))
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });
        }

        services.AddAuthorization(); // sin roles/políticas
        return services;
    }
}