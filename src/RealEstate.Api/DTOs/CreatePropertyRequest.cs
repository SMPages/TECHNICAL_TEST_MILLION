using System.ComponentModel.DataAnnotations;

namespace RealEstate.Api.DTOs;

public sealed class CreatePropertyRequest
{
    [Required, StringLength(50)]
    public string Code { get; init; } = default!;

    [Required, StringLength(150)]
    public string Name { get; init; } = default!;

    [Required, StringLength(300)]
    public string Address { get; init; } = default!;

    [StringLength(100)]
    public string? City { get; init; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    [Range(1, int.MaxValue)]
    public int IdOwner { get; init; }

    public short? Year { get; init; }
    public byte? Bedrooms { get; init; }
    public byte? Bathrooms { get; init; }
    public double? AreaSqFt { get; init; }

    [StringLength(1000)]
    public string? Description { get; init; }
}