using System.ComponentModel.DataAnnotations;

namespace RealEstate.Api.DTOs;

public sealed class AddImageRequest
{
    [Required, StringLength(300)] public string FileUrl { get; init; } = default!;
    public bool IsMain { get; init; }
    [StringLength(200)] public string? Caption { get; init; }
    public int SortOrder { get; init; }
}