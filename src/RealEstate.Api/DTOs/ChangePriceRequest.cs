using System.ComponentModel.DataAnnotations;

namespace RealEstate.Api.DTOs;

public sealed class ChangePriceRequest
{
    [Range(0.01, double.MaxValue)] public decimal NewPrice { get; init; }
    [StringLength(500)] public string? Notes { get; init; }
}