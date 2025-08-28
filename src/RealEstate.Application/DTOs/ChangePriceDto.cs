namespace RealEstate.Application.DTOs;

public sealed class ChangePriceDto
{
    public decimal NewPrice { get; init; }
    public string? Notes { get; init; }
}