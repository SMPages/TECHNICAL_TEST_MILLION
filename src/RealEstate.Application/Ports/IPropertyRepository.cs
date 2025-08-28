using RealEstate.Domain.Entities;

namespace RealEstate.Application.Ports;

public interface IPropertyRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<Property?> GetByIdAsync(int id, bool asNoTracking, CancellationToken ct);
    Task<Property?> GetByCodeAsync(string code, bool asNoTracking, CancellationToken ct);
    Task AddAsync(Property property, CancellationToken ct);
    Task UpdateAsync(Property property, CancellationToken ct);
    Task AddImageAsync(PropertyImage image, CancellationToken ct);
    Task AddTraceAsync(PropertyTrace trace, CancellationToken ct);

    Task<(IReadOnlyList<Property> Items, int Total)> ListAsync(
        string? city, decimal? minPrice, decimal? maxPrice, byte? bedrooms, byte? bathrooms,
        int page, int pageSize, CancellationToken ct);
}
