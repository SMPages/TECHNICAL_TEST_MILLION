using Microsoft.Extensions.Logging;
using RealEstate.Application.Ports;

namespace RealEstate.Application.UseCases;

/// <summary>
/// Caso de uso para actualizar datos básicos de una propiedad.
/// 
public sealed class UpdateProperty
{
    private readonly IPropertyRepository _repo;
    private readonly ILogger<UpdateProperty> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="UpdateProperty"/>.
    /// </summary>
    public UpdateProperty(IPropertyRepository repo, ILogger<UpdateProperty> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        int id,
        string name,
        string address,
        string? city,
        short? year,
        byte? bedrooms,
        byte? bathrooms,
        double? area,
        string? description,
        CancellationToken ct = default)
    {
        // 1) Cargar
        var p = await _repo.GetByIdAsync(id, asNoTracking: false, ct)
                ?? throw new KeyNotFoundException("Property not found");

        // 2) Actualizar datos básicos (firma correcta: 7 argumentos)
        p.UpdateBasicInfo(name, address, city, year, bedrooms, bathrooms, area);

        // 3) Persistir
        await _repo.UpdateAsync(p, ct);

        _logger.LogInformation(
            "Property updated. Id={Id}, City={City}, Year={Year}, Beds={Bedrooms}, Baths={Bathrooms}, Area={Area}",
            id, city, year, bedrooms, bathrooms, area);
    }
}