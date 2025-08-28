namespace RealEstate.Api.DTOs;

public sealed class ListResponse<T>
{
    public int Total { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}