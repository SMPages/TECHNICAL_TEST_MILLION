using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RealEstate.Tests.Infra;

/// <summary>
/// Helper para tests de integración:
/// - Obtiene un JWT desde /api/Auth/GenerateToken.
/// - Inyecta el header Authorization: Bearer {token}.
/// </summary>
public static class TestClientHelper
{
    /// <summary>
    /// Llama al endpoint de Auth y devuelve el accessToken del JSON.
    /// Usuarios demo: admin/admin123 (configurable por parámetros).
    /// </summary>
    public static async Task<string> GetJwtAsync(HttpClient client, string username = "admin", string password = "admin123")
    {
        var resp = await client.PostAsJsonAsync("/api/Auth/GenerateToken", new { username, password });
        resp.EnsureSuccessStatusCode();

        using var stream = await resp.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.GetProperty("accessToken").GetString()!;
    }

    /// <summary>
    /// Setea el header Authorization con el Bearer token indicado.
    /// </summary>
    public static void WithBearer(this HttpClient client, string token)
        => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
