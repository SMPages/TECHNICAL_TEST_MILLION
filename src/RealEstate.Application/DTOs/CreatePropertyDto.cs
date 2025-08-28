namespace RealEstate.Application.DTOs;
public sealed class CreatePropertyDto
{
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string? City { get; init; }
    public decimal Price { get; init; }
    public int IdOwner { get; init; }
    public short? Year { get; init; }
    public byte? Bedrooms { get; init; }
    public byte? Bathrooms { get; init; }
    public double? AreaSqFt { get; init; }
    public string? Description { get; init; }
}
