using System.ComponentModel.DataAnnotations;

namespace RealEstate.Api.DTOs.Auth;

public sealed class LoginRequest
{
    [Required] public string Username { get; init; } = default!;
    [Required] public string Password { get; init; } = default!;
}