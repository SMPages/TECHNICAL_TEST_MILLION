using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Infrastructure.Persistence;
using EF = RealEstate.Infrastructure.Persistence.EF.Models;

namespace RealEstate.Tests.Infra;

public class TestWebAppFactory : WebApplicationFactory<RealEstate.Api.Program>
{
    private static readonly InMemoryDatabaseRoot _root = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var toRemove = services.Where(d =>
                   d.ServiceType == typeof(DbContextOptions<RealEstateDbContext>)
                || d.ServiceType == typeof(RealEstateDbContext)
                || d.ServiceType == typeof(IDbContextFactory<RealEstateDbContext>))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            services.AddDbContext<RealEstateDbContext>(opt =>
                opt.UseInMemoryDatabase("RealEstate_TestDb", _root)
                   .EnableSensitiveDataLogging());

            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RealEstateDbContext>();

            db.Database.EnsureCreated(); // ✅ NO EnsureDeleted

            if (!db.Owners.Any())
            {
                db.Owners.AddRange(
                    new EF.Owner { Name = "John Doe" },
                    new EF.Owner { Name = "Mary Smith" }
                );
                db.SaveChanges();
            }

            var ownerId = db.Owners.Select(o => o.IdOwner).First();

            if (!db.Properties.Any())
            {
                db.Properties.AddRange(
                    new EF.Property { CodeInternal = "P-001", Name = "Apto Centro", Address = "Dir 1", City = "Bogotá", Price = 250000, IdOwner = ownerId },
                    new EF.Property { CodeInternal = "P-002", Name = "Casa Norte", Address = "Dir 2", City = "Bogotá", Price = 380000, Bedrooms = 3, Bathrooms = 2, AreaSqFt = 120, IdOwner = ownerId }
                );
                db.SaveChanges();
            }
        });
    }
}
