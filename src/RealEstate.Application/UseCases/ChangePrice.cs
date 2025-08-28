using Microsoft.Extensions.Logging;
using RealEstate.Application.Ports;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.UseCases;

/// <summary>
/// Caso de uso para cambiar el precio de una propiedad.
/// 
public sealed class ChangePrice
{
    private readonly IPropertyRepository _repo;
    private readonly ILogger<ChangePrice> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ChangePrice"/>.
    /// </summary>
    public ChangePrice(IPropertyRepository repo, ILogger<ChangePrice> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta la operación de cambio de precio sobre una propiedad.
    /// </summary>
    public async Task ExecuteAsync(int idProperty, decimal newPrice, string? notes, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync(idProperty, asNoTracking: false, ct)
                 ?? throw new KeyNotFoundException("Property not found");

        p.ChangePrice(newPrice);
        await _repo.UpdateAsync(p, ct);

        await _repo.AddTraceAsync(new PropertyTrace(idProperty, "PRICE_CHANGE", newPrice, 0, null), ct);

        _logger.LogInformation("Price changed for PropertyId={PropertyId} NewPrice={NewPrice} Notes={Notes}",
            idProperty, newPrice, notes);
    }
}
