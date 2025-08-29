using FluentAssertions;
using RealEstate.Tests.Infra;             // TestWebAppFactory, TestClientHelper
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RealEstate.Tests.Integration
{
    /// <summary>
    /// Pruebas de integración sobre la API real (host de test con EF InMemory).
    /// Verifica Auth y endpoints de Properties (crear, actualizar, listar, etc.).
    /// </summary>
    [TestFixture]
    public class ApiIntegrationTests
    {
        private TestWebAppFactory _factory = null!;
        private HttpClient _client = null!;

        [SetUp]
        public void SetUp()
        {
            _factory = new TestWebAppFactory();
            _client = _factory.CreateClient(); // Host interno (sin levantar servidor externo)
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        /// <summary>Auth: debería devolver 200 y un accessToken válido.</summary>
        [Test]
        public async Task Auth_GenerateToken_Returns200AndToken()
        {
            var resp = await _client.PostAsJsonAsync("/api/Auth/GenerateToken", new { username = "admin", password = "admin123" });
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            json.RootElement.TryGetProperty("accessToken", out var tokenProp).Should().BeTrue();
            tokenProp.GetString().Should().NotBeNullOrWhiteSpace();
        }

        /// <summary>Properties: la lista pública debería responder 200 y total &gt; 0.</summary>
        [Test]
        public async Task Properties_List_Anonymous_ReturnsOk()
        {
            var resp = await _client.GetAsync("/api/Properties/ListProperties?city=Bogotá&page=1&pageSize=5");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            json.RootElement.TryGetProperty("total", out var totalProp).Should().BeTrue();
            totalProp.GetInt32().Should().BeGreaterThan(0);
        }

        /// <summary>Crea una propiedad con token y luego verifica que aparece en la lista.</summary>
        [Test]
        public async Task Properties_Create_WithToken_ReturnsCreated_Then_ListContainsIt()
        {
            var token = await TestClientHelper.GetJwtAsync(_client);
            _client.WithBearer(token);

            var body = new
            {
                code = "P-777",
                name = "Casa Pruebas",
                address = "Calle Test #1-2",
                city = "Bogotá",
                price = 123456,
                idOwner = 1,
                year = 2024,
                bedrooms = 3,
                bathrooms = 2,
                areaSqFt = 111,
                description = "Creada por test de integración"
            };

            var create = await _client.PostAsJsonAsync("/api/Properties/CreateProperty", body);
            create.StatusCode.Should().Be(HttpStatusCode.Created);

            var list = await _client.GetAsync("/api/Properties/ListProperties?city=Bogotá&minPrice=100000&page=1&pageSize=50");
            list.EnsureSuccessStatusCode();

            var json = JsonDocument.Parse(await list.Content.ReadAsStringAsync());
            var items = json.RootElement.GetProperty("items");
            items.EnumerateArray().Any(e => e.GetProperty("codeInternal").GetString() == "P-777").Should().BeTrue();
        }

        /// <summary>ChangePrice: con token, debería devolver 204 NoContent.</summary>
        [Test]
        public async Task Properties_ChangePrice_WithToken_ReturnsNoContent()
        {
            var token = await TestClientHelper.GetJwtAsync(_client);
            _client.WithBearer(token);

            var change = await _client.PutAsJsonAsync("/api/Properties/ChangePrice/1", new { newPrice = 999999, notes = "Test change" });
            change.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        /// <summary>UpdateProperty: con token, debería devolver 204 NoContent.</summary>
        [Test]
        public async Task Properties_Update_WithToken_ReturnsNoContent()
        {
            var token = await TestClientHelper.GetJwtAsync(_client);
            _client.WithBearer(token);

            var update = await _client.PutAsJsonAsync("/api/Properties/UpdateProperty/1", new
            {
                name = "Apto Centro Edit",
                address = "Dir 1 Edit",
                city = "Bogotá",
                year = 2022,
                bedrooms = 2,
                bathrooms = 1,
                areaSqFt = 80,
                description = "Editado por test"
            });
            update.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        /// <summary>AddImageToProperty: con token, debería devolver 200 OK.</summary>
        [Test]
        public async Task Properties_AddImage_WithToken_ReturnsOk()
        {
            var token = await TestClientHelper.GetJwtAsync(_client);
            _client.WithBearer(token);

            var img = await _client.PostAsJsonAsync("/api/Properties/1/AddImageToProperty", new
            {
                fileUrl = "/uploads/images/test-int.jpg",
                isMain = true,
                caption = "Test img",
                sortOrder = 1
            });

            img.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>CreateProperty: sin token debe fallar con 401 Unauthorized.</summary>
        [Test]
        public async Task Properties_Create_Unauthorized_WithoutToken()
        {
            var create = await _client.PostAsJsonAsync("/api/Properties/CreateProperty", new
            {
                code = "NO-TOKEN",
                name = "DebeFallar",
                address = "N/A",
                city = "N/A",
                price = 1,
                idOwner = 1
            });

            create.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
