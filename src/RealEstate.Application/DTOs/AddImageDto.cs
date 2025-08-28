namespace RealEstate.Application.DTOs;

public sealed class AddImageDto
{
    public string FileUrl { get; init; } = default!;
    public bool IsMain { get; init; }
    public string? Caption { get; init; }
    public int SortOrder { get; init; }
}