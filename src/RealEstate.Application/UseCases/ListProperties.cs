using RealEstate.Application.Ports;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.UseCases;

public sealed class ListProperties
{
    private readonly IPropertyRepository _repo;
    public ListProperties(IPropertyRepository repo) => _repo = repo;

    public Task<(IReadOnlyList<Property> Items, int Total)> ExecuteAsync(
        string? city, decimal? minPrice, decimal? maxPrice, byte? bedrooms, byte? bathrooms,
        int page = 1, int pageSize = 20, CancellationToken ct = default)
        => _repo.ListAsync(city, minPrice, maxPrice, bedrooms, bathrooms, page, pageSize, ct);
}