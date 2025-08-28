using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Ports;
using RealEstate.Application.Services;
using RealEstate.Application.UseCases;

namespace RealEstate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use cases
        services.AddScoped<CreateProperty>();
        services.AddScoped<AddImage>();
        services.AddScoped<ChangePrice>();
        services.AddScoped<UpdateProperty>();
        services.AddScoped<ListProperties>();

        // Facade
        services.AddScoped<IPropertiesService, PropertiesService>();
        return services;
    }
}