namespace RealEstate.Application.DTOs;

public sealed class UpdatePropertyDto
{
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string? City { get; init; }
    public short? Year { get; init; }
    public byte? Bedrooms { get; init; }
    public byte? Bathrooms { get; init; }
    public double? AreaSqFt { get; init; }
    public string? Description { get; init; }
}