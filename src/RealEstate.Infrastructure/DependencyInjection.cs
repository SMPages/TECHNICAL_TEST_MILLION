using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstate.Application.Ports;
using RealEstate.Infrastructure.Mapping;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Repositories;

namespace RealEstate.Infrastructure;

/// <summary>
/// Extensiones de DI para registrar la infraestructura (EF Core, repositorios, AutoMapper).
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega los servicios de infraestructura al contenedor.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
     this IServiceCollection services,
     IConfiguration config,
     IHostEnvironment env)
    {
        var already = services.Any(d =>
               d.ServiceType == typeof(DbContextOptions<RealEstateDbContext>)
            || d.ServiceType == typeof(RealEstateDbContext)
            || d.ServiceType == typeof(IDbContextFactory<RealEstateDbContext>));

        if (!env.IsEnvironment("Testing") && !already)
        {
            services.AddDbContext<RealEstateDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("RealEstate")));
        }

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddAutoMapper((Action<AutoMapper.IMapperConfigurationExpression>)null, typeof(EfDomainProfile));
        return services;
    }
}
