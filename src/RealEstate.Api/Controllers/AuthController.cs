using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Api.DTOs.Auth;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration config, ILogger<AuthController> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>Emite un JWT para consumir la API.</summary>
    [HttpPost("GenerateToken")]
    [SwaggerOperation(Summary = "Get Token", Description = "Devuelve un JWT si el usuario/clave son válidos.")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult Token([FromBody] LoginRequest req)
    {
        // DEMO: validación mínima. Reemplaza con tu store/Identity/DB.
        var users = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["admin"] = "admin123",
            ["agent"] = "agent123",
            ["viewer"] = "viewer123"
        };

        if (!users.TryGetValue(req.Username, out var pwd) || pwd != req.Password)
        {
            _logger.LogWarning("Bad credentials for {User}", req.Username);
            return Unauthorized(new { error = "Invalid credentials" });
        }

        var now = DateTime.UtcNow;

        var keyStr = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(keyStr))
        {
            _logger.LogError("JWT Key is not configured (Jwt:Key).");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "JWT configuration missing" });
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, req.Username),
            new Claim(ClaimTypes.Name, req.Username),
            // sin roles
            new Claim("scope", "properties.read properties.write")
        };

        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var minutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(minutes),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation("Issued token for {User}", req.Username);

        return Ok(new TokenResponse
        {
            AccessToken = jwt,
            ExpiresAtUtc = token.ValidTo
        });
    }
}
