using Microsoft.Extensions.Logging;
using RealEstate.Application.Ports;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.UseCases;

/// <summary>
/// Caso de uso para crear una nueva propiedad.
/// 
public sealed class CreateProperty
{
    private readonly IPropertyRepository _repo;
    private readonly ILogger<CreateProperty> _logger;
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="CreateProperty"/>.
    /// </summary>
    public CreateProperty(IPropertyRepository repo, ILogger<CreateProperty> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta la operación de creación de propiedad.
    /// </summary>
    public async Task<int> ExecuteAsync(
            string code, string name, string address, string? city,
            decimal price, int ownerId, short? year = null, byte? bedrooms = null,
            byte? bathrooms = null, double? area = null, string? description = null, // se ignora por ahora
            CancellationToken ct = default)
    {
        // 1) Verificar unicidad de CodeInternal
        if (await _repo.CodeExistsAsync(code, ct))
        {
            _logger.LogWarning("CreateProperty conflict: Code already exists. Code={Code}", code);
            throw new InvalidOperationException("Code already exists");
        }

        // 2) Construir entidad 
        var property = new Property(code, name, address, ownerId, price);

        // 3) Actualizar datos
        property.UpdateBasicInfo(name, address, city, year, bedrooms, bathrooms, area);

        // 4) Guardar datos
        await _repo.AddAsync(property, ct);

        // 5) Log de auditoría
        _logger.LogInformation("Property created successfully. Id={Id} Code={Code}", property.IdProperty, code);

        return property.IdProperty;
    }
}