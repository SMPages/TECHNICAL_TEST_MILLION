using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealEstate.Api.Setup;

/// <summary>
/// Registro y configuración de Swagger/OpenAPI para la API.
/// </summary>
public static class SwaggerSetup
{
    /// <summary>
    /// Agrega e inicializa Swagger/OpenAPI al contenedor de servicios.
    /// </summary>
    public static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.CustomOperationIds(api => api.TryGetMethodInfo(out var mi) ? mi.Name : null);

            var xml = Path.Combine(AppContext.BaseDirectory, "RealEstate.Api.xml");
            if (File.Exists(xml))
                c.IncludeXmlComments(xml, includeControllerXmlComments: true);


            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RealEstate API – Million",
                Version = "v1",
                Description =
                   "API para gestión de propiedades (crear, actualizar, cambiar precio, imágenes y listados con filtros). " +
                   "\n\n**Autenticación**: Bearer JWT vía `/api/Auth/GenerateToken`.",
                Contact = new OpenApiContact
                {
                    Name = "Greydy Sebastián Marciales Rubio",
                    Email = "sebastianmarciales40@gmail.com",
                    Url = new Uri("https://dev-sebas.com/")
                }
            });
            // JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization. Usar: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
    /// <summary>
    /// Habilita Swagger y Swagger UI según el entorno.
    /// </summary>
    public static IApplicationBuilder UseAppSwaggerUI(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}