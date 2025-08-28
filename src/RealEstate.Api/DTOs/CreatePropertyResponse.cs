namespace RealEstate.Api.DTOs;

public sealed class CreatePropertyResponse
{
    public int Id { get; init; }
    public string Code { get; init; } = default!;
}