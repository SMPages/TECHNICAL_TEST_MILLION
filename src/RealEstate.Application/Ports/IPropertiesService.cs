using RealEstate.Application.DTOs;

namespace RealEstate.Application.Ports;

public interface IPropertiesService
{
    Task<int> CreateAsync(CreatePropertyDto dto, CancellationToken ct);
    Task<int> AddImageAsync(int propertyId, AddImageDto dto, CancellationToken ct);
    Task ChangePriceAsync(int propertyId, ChangePriceDto dto, CancellationToken ct);
    Task UpdateAsync(int propertyId, UpdatePropertyDto dto, CancellationToken ct);
    Task<(IReadOnlyList<PropertyListItemDto> Items, int Total)>
            ListAsync(ListPropertiesQueryDto query, CancellationToken ct);
}
