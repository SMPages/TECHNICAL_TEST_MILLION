namespace RealEstate.Application.DTOs;

public sealed class ListPropertiesQueryDto
{
    public string? City { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public byte? Bedrooms { get; init; }
    public byte? Bathrooms { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}