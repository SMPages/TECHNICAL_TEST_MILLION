using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealEstate.Infrastructure.Mapping;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Tests.Util
{
    /// <summary>
    /// Fabrica un DbContext EF InMemory (DB aislada por test) y un IMapper con el perfil real.
    /// </summary>
    internal static class TestDb
    {
        /// <summary>
        /// Crea una BD efímera y un mapper configurado con <see cref="EfDomainProfile"/>.
        /// </summary>
        public static (RealEstateDbContext Db, IMapper Mapper) CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<RealEstateDbContext>()
                .UseInMemoryDatabase($"RealEstateDbTests_{Guid.NewGuid()}") // nombre único por ejecución
                .EnableSensitiveDataLogging()
                .Options;

            var db = new RealEstateDbContext(options);

            // AutoMapper v15: ctor público requiere loggerFactory.
            var mapperConfig = new MapperConfiguration(
                cfg => { cfg.AddProfile<EfDomainProfile>(); },
                new NullLoggerFactory()
            );

            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();

            return (db, mapper);
        }
    }
}
