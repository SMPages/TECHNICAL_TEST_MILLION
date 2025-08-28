using RealEstate.Application.DTOs;
using RealEstate.Application.Ports;
using RealEstate.Application.UseCases;

namespace RealEstate.Application.Services;

/// <summary>
/// Fachada de casos de uso relacionados con Propiedades.
/// Delegamos en los use cases existentes para mantenerlos reutilizables y testeables.
/// </summary>
public sealed class PropertiesService : IPropertiesService
{
    private readonly CreateProperty _create;
    private readonly AddImage _addImage;
    private readonly ChangePrice _changePrice;
    private readonly UpdateProperty _update;
    private readonly ListProperties _list;

    public PropertiesService(
        CreateProperty create,
        AddImage addImage,
        ChangePrice changePrice,
        UpdateProperty update,
        ListProperties list)
    {
        _create = create;
        _addImage = addImage;
        _changePrice = changePrice;
        _update = update;
        _list = list;
    }

    public Task<int> CreateAsync(CreatePropertyDto dto, CancellationToken ct) =>
        _create.ExecuteAsync(dto.Code, dto.Name, dto.Address, dto.City, dto.Price, dto.IdOwner,
                             dto.Year, dto.Bedrooms, dto.Bathrooms, dto.AreaSqFt, dto.Description, ct);

    public Task<int> AddImageAsync(int propertyId, AddImageDto dto, CancellationToken ct) =>
        _addImage.ExecuteAsync(propertyId, dto.FileUrl, dto.IsMain, dto.Caption, dto.SortOrder, ct);

    public Task ChangePriceAsync(int propertyId, ChangePriceDto dto, CancellationToken ct) =>
        _changePrice.ExecuteAsync(propertyId, dto.NewPrice, dto.Notes, ct);

    public Task UpdateAsync(int propertyId, UpdatePropertyDto dto, CancellationToken ct) =>
        _update.ExecuteAsync(propertyId, dto.Name, dto.Address, dto.City, dto.Year,
                             dto.Bedrooms, dto.Bathrooms, dto.AreaSqFt, dto.Description, ct);

    public async Task<(IReadOnlyList<PropertyListItemDto> Items, int Total)>
        ListAsync(ListPropertiesQueryDto q, CancellationToken ct)
    {
        var (entities, total) = await _list.ExecuteAsync(
            q.City, q.MinPrice, q.MaxPrice, q.Bedrooms, q.Bathrooms, q.Page, q.PageSize, ct);

        // Mapear Domain -> DTO (sin exponer entidades)
        var items = entities.Select(p => new PropertyListItemDto
        {
            IdProperty = p.IdProperty,
            CodeInternal = p.CodeInternal,
            Name = p.Name,
            Address = p.Address,
            City = p.City,
            Price = p.Price,
            Year = p.Year,
            Bedrooms = p.Bedrooms,
            Bathrooms = p.Bathrooms,
            AreaSqFt = p.AreaSqFt,
            IdOwner = p.IdOwner,
            CreatedAt = p.CreatedAt
        }).ToList();

        return (items, total);
    }
}
