using System.Linq;                                // First(), Any(), All()
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;                            // [Test], [SetUp], etc.
using RealEstate.Infrastructure.Repositories;
using RealEstate.Tests.Util;

// Dominio
using DProperty = RealEstate.Domain.Entities.Property;
using DPropertyImage = RealEstate.Domain.Entities.PropertyImage;
// EF
using EProperty = RealEstate.Infrastructure.Persistence.EF.Models.Property;

namespace RealEstate.Tests.Repositories
{
    /// <summary>
    /// Pruebas unitarias del PropertyRepository con EF InMemory:
    /// CodeExists, Add, Update, List y AddImage.
    /// </summary>
    [TestFixture]
    public class PropertyRepositoryTests
    {
        private RealEstate.Infrastructure.Persistence.RealEstateDbContext _db = null!;
        private IMapper _mapper = null!;
        private PropertyRepository _repo = null!;

        [SetUp]
        public void SetUp()
        {
            (_db, _mapper) = TestDb.CreateInMemory();
            _repo = new PropertyRepository(_db, new NullLogger<PropertyRepository>(), _mapper);

            // Seed mínimo (Owners + Properties) con modelos EF
            _db.Owners.AddRange(
                new RealEstate.Infrastructure.Persistence.EF.Models.Owner { Name = "John Doe" },
                new RealEstate.Infrastructure.Persistence.EF.Models.Owner { Name = "Mary Smith" }
            );
            _db.SaveChanges();

            var owner1 = _db.Owners.First().IdOwner;

            _db.Properties.AddRange(
                new EProperty { CodeInternal = "P-001", Name = "Apto Centro", Address = "Dir 1", City = "Bogotá", Price = 250000, IdOwner = owner1 },
                new EProperty { CodeInternal = "P-002", Name = "Casa Norte", Address = "Dir 2", City = "Bogotá", Price = 380000, Bedrooms = 3, Bathrooms = 2, AreaSqFt = 120, IdOwner = owner1 },
                new EProperty { CodeInternal = "P-003", Name = "Apto Poblado", Address = "Dir 3", City = "Medellín", Price = 420000, IdOwner = owner1 }
            );
            _db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();
        }

        /// <summary>Debe devolver true si el código existe.</summary>
        [Test]
        public async Task CodeExistsAsync_ReturnsTrue_WhenCodeExists()
        {
            var exists = await _repo.CodeExistsAsync("P-001", CancellationToken.None);
            Assert.That(exists, Is.True);
        }

        /// <summary>Debe devolver false si el código no existe.</summary>
        [Test]
        public async Task CodeExistsAsync_ReturnsFalse_WhenCodeNotFound()
        {
            var exists = await _repo.CodeExistsAsync("NO-EXISTE", CancellationToken.None);
            Assert.That(exists, Is.False);
        }

        /// <summary>Agregar una propiedad debe persistir y asignar Id.</summary>
        [Test]
        public async Task AddAsync_PersistsAndAssignsId()
        {
            // Dominio: ctor real (sin city) + UpdateBasicInfo
            var d = new DProperty(
                codeInternal: "P-100",
                name: "Casa Campestre",
                address: "Km 5 Vía La Calera",
                idOwner: _db.Owners.First().IdOwner,
                price: 550000m
            );

            d.UpdateBasicInfo(
                name: d.Name,
                address: d.Address,
                city: "Bogotá",
                year: (short)2020,
                bedrooms: (byte)4,
                bathrooms: (byte)3,
                areaSqFt: 180d
            );

            await _repo.AddAsync(d, CancellationToken.None);

            var ef = await _db.Properties.AsNoTracking().SingleAsync(p => p.CodeInternal == "P-100");
            Assert.That(ef.IdProperty, Is.GreaterThan(0));
        }

        /// <summary>Actualizar debe guardar cambios (precio, beds...).</summary>
        [Test]
        public async Task UpdateAsync_ChangesAreSaved()
        {
            var ef = await _db.Properties.AsNoTracking()
                .SingleAsync(p => p.CodeInternal == "P-002");

            var d = new DProperty(
                codeInternal: ef.CodeInternal!,
                name: "Casa Norte Remodelada",
                address: ef.Address!,
                idOwner: ef.IdOwner,
                price: 400000m               // cambio de precio
            );

            d.UpdateBasicInfo(
                name: "Casa Norte Remodelada",
                address: ef.Address!,
                city: ef.City,
                year: ef.Year.HasValue ? (short)ef.Year.Value : (short?)null,
                bedrooms: (byte?)4,
                bathrooms: (byte?)3,
                areaSqFt: ef.AreaSqFt.HasValue ? (double)ef.AreaSqFt.Value : (double?)150d
            );

            // setter internal -> reflection para setear Id
            typeof(DProperty).GetProperty("IdProperty")!.SetValue(d, ef.IdProperty);

            await _repo.UpdateAsync(d, CancellationToken.None);

            var updated = await _db.Properties.AsNoTracking()
                .SingleAsync(p => p.IdProperty == ef.IdProperty);

            Assert.Multiple(() =>
            {
                Assert.That(updated.Price, Is.EqualTo(400000m));
                Assert.That(updated.Bedrooms, Is.EqualTo((byte?)4));
            });
        }

        /// <summary>Listar con filtros debe aplicar city y bedrooms.</summary>
        [Test]
        public async Task ListAsync_AppliesCityAndBedroomsFilters()
        {
            var (items, total) = await _repo.ListAsync(
                city: "Bogotá",
                minPrice: 200000m,
                maxPrice: 400000m,
                bedrooms: (byte?)2,
                bathrooms: null,
                page: 1,
                pageSize: 10,
                ct: CancellationToken.None
            );

            Assert.Multiple(() =>
            {
                Assert.That(total, Is.GreaterThanOrEqualTo(1));
                Assert.That(items.All(p => p.City == "Bogotá"), "Todas deben ser de Bogotá");
                Assert.That(items.All(p => p.Bedrooms is null or >= 2), "Dormitorios >= 2 o null");
            });
        }

        /// <summary>Agregar imagen debe persistir vinculada a la propiedad.</summary>
        [Test]
        public async Task AddImageAsync_PersistsImageLinkedToProperty()
        {
            var prop = await _db.Properties.AsNoTracking().FirstAsync();

            var img = new DPropertyImage(
                idProperty: prop.IdProperty,
                fileUrl: "/uploads/images/test.jpg",
                isMain: true,
                caption: "Fachada",
                sortOrder: 1
            );

            await _repo.AddImageAsync(img, CancellationToken.None);

            var stored = await _db.PropertyImages.AsNoTracking()
                .FirstOrDefaultAsync(i => i.IdProperty == prop.IdProperty && i.FileUrl == "/uploads/images/test.jpg");

            Assert.That(stored, Is.Not.Null);
        }
    }
}
