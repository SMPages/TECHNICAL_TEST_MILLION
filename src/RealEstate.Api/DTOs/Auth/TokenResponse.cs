namespace RealEstate.Api.DTOs.Auth;

public sealed class TokenResponse
{
    public string AccessToken { get; init; } = default!;
    public DateTime ExpiresAtUtc { get; init; }
}