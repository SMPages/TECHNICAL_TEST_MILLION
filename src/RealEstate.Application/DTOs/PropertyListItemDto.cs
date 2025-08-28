namespace RealEstate.Application.DTOs;

public sealed class PropertyListItemDto
{
    public int IdProperty { get; init; }
    public string CodeInternal { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string? City { get; init; }
    public decimal Price { get; init; }
    public short? Year { get; init; }
    public byte? Bedrooms { get; init; }
    public byte? Bathrooms { get; init; }
    public double? AreaSqFt { get; init; }
    public int IdOwner { get; init; }
    public DateTime CreatedAt { get; init; }
}