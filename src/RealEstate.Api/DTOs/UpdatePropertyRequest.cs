using System.ComponentModel.DataAnnotations;

namespace RealEstate.Api.DTOs;

public sealed class UpdatePropertyRequest
{
    [Required, StringLength(150)] public string Name { get; init; } = default!;
    [Required, StringLength(300)] public string Address { get; init; } = default!;
    [StringLength(100)] public string? City { get; init; }
    public short? Year { get; init; }
    public byte? Bedrooms { get; init; }
    public byte? Bathrooms { get; init; }
    public double? AreaSqFt { get; init; }
    [StringLength(1000)] public string? Description { get; init; }
}