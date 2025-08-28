using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Ports;
using RealEstate.Infrastructure.Mapping;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Repositories;

namespace RealEstate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<RealEstateDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("RealEstate")));

        services.AddScoped<IPropertyRepository, PropertyRepository>();

        // AutoMapper v15: pasa null como configAction y el marcador de perfil
        services.AddAutoMapper(
            (Action<AutoMapper.IMapperConfigurationExpression>)null,
            typeof(EfDomainProfile)
        );

        return services;
    }
}
