using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RealEstate.Application.Ports;
using RealEstate.Infrastructure.Persistence;

// Aliases Dominio
using DProperty = RealEstate.Domain.Entities.Property;
using DPropertyImage = RealEstate.Domain.Entities.PropertyImage;
using DPropertyTrace = RealEstate.Domain.Entities.PropertyTrace;

// Aliases EF
using EProperty = RealEstate.Infrastructure.Persistence.EF.Models.Property;
using EPropertyImage = RealEstate.Infrastructure.Persistence.EF.Models.PropertyImage;
using EPropertyTrace = RealEstate.Infrastructure.Persistence.EF.Models.PropertyTrace;

namespace RealEstate.Infrastructure.Repositories;

/// <summary>
/// Repositorio EF Core para la agregación <see cref="DProperty"/> con AutoMapper.
/// Mantiene manejo de errores y logging detallado.
/// </summary>
public sealed class PropertyRepository : IPropertyRepository
{
    private readonly RealEstateDbContext _db;
    private readonly ILogger<PropertyRepository> _logger;
    private readonly IMapper _mapper;

    public PropertyRepository(RealEstateDbContext db, ILogger<PropertyRepository> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct)
    {
        try
        {
            return await _db.Set<EProperty>()
                .AsNoTracking()
                .AnyAsync(p => p.CodeInternal == code, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking CodeInternal existence. Code={Code}", code);
            throw;
        }
    }

    public async Task<DProperty?> GetByIdAsync(int id, bool asNoTracking, CancellationToken ct)
    {
        try
        {
            IQueryable<EProperty> q = _db.Set<EProperty>();
            if (asNoTracking) q = q.AsNoTracking();

            var ef = await q.FirstOrDefaultAsync(p => p.IdProperty == id, ct);
            var dom = _mapper.Map<DProperty?>(ef);

            _logger.LogDebug("GetById completed. Id={Id} Found={Found}", id, dom is not null);
            return dom;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting property by Id. Id={Id}", id);
            throw;
        }
    }

    public async Task<DProperty?> GetByCodeAsync(string code, bool asNoTracking, CancellationToken ct)
    {
        try
        {
            IQueryable<EProperty> q = _db.Set<EProperty>();
            if (asNoTracking) q = q.AsNoTracking();

            var ef = await q.FirstOrDefaultAsync(p => p.CodeInternal == code, ct);
            var dom = _mapper.Map<DProperty?>(ef);

            _logger.LogDebug("GetByCode completed. Code={Code} Found={Found}", code, dom is not null);
            return dom;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting property by CodeInternal. Code={Code}", code);
            throw;
        }
    }

    public async Task AddAsync(DProperty property, CancellationToken ct)
    {
        try
        {
            var ef = _mapper.Map<EProperty>(property);
            _db.Set<EProperty>().Add(ef);
            await _db.SaveChangesAsync(ct);

            // Si el Id es identity, reflejarlo en dominio
            property.IdProperty = ef.IdProperty;

            _logger.LogInformation("Property added. Id={Id} Code={Code}", property.IdProperty, property.CodeInternal);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "DbUpdateException adding property. Code={Code}", property.CodeInternal);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding property. Code={Code}", property.CodeInternal);
            throw;
        }
    }

    public async Task UpdateAsync(DProperty property, CancellationToken ct)
    {
        try
        {
            var ef = await _db.Set<EProperty>().FirstOrDefaultAsync(p => p.IdProperty == property.IdProperty, ct);
            if (ef is null)
            {
                _logger.LogWarning("Property not found for update. Id={Id}", property.IdProperty);
                throw new KeyNotFoundException($"Property {property.IdProperty} not found.");
            }

            // Aplica los cambios del dominio sobre la instancia rastreada
            _mapper.Map(property, ef);

            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Property updated. Id={Id}", property.IdProperty);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency error updating property. Id={Id}", property.IdProperty);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "DbUpdateException updating property. Id={Id}", property.IdProperty);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating property. Id={Id}", property.IdProperty);
            throw;
        }
    }

    public async Task AddImageAsync(DPropertyImage image, CancellationToken ct)
    {
        try
        {
            var ef = _mapper.Map<EPropertyImage>(image);
            _db.Set<EPropertyImage>().Add(ef);
            await _db.SaveChangesAsync(ct);

            image.IdPropertyImage = ef.IdPropertyImage;

            _logger.LogInformation("Property image added. PropertyId={PropertyId} ImageId={ImageId}",
                image.IdProperty, image.IdPropertyImage);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "DbUpdateException adding image. PropertyId={PropertyId}", image.IdProperty);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding image. PropertyId={PropertyId}", image.IdProperty);
            throw;
        }
    }

    public async Task AddTraceAsync(DPropertyTrace trace, CancellationToken ct)
    {
        try
        {
            var ef = _mapper.Map<EPropertyTrace>(trace);
            _db.Set<EPropertyTrace>().Add(ef);
            await _db.SaveChangesAsync(ct);

            trace.IdPropertyTrace = ef.IdPropertyTrace;

            _logger.LogInformation("Property trace added. PropertyId={PropertyId} Name={Name} Value={Value}",
                trace.IdProperty, trace.Name, trace.Value);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "DbUpdateException adding trace. PropertyId={PropertyId}", trace.IdProperty);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding trace. PropertyId={PropertyId}", trace.IdProperty);
            throw;
        }
    }

    public async Task<(IReadOnlyList<DProperty> Items, int Total)> ListAsync(
        string? city, decimal? minPrice, decimal? maxPrice, byte? bedrooms, byte? bathrooms,
        int page, int pageSize, CancellationToken ct)
    {
        try
        {
            var q = _db.Set<EProperty>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(city)) q = q.Where(p => p.City == city);
            if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice.Value);
            if (bedrooms.HasValue) q = q.Where(p => p.Bedrooms >= bedrooms.Value);
            if (bathrooms.HasValue) q = q.Where(p => p.Bathrooms >= bathrooms.Value);

            var total = await q.CountAsync(ct);

            // Proyección a Dominio en SQL (eficiente)
            var items = await q
                .OrderByDescending(p => p.IdProperty)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<DProperty>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            _logger.LogDebug("Listed properties. City={City}, Min={Min}, Max={Max}, Beds>={Beds}, Baths>={Baths}. Page={Page}, Size={Size}, Total={Total}",
                city, minPrice, maxPrice, bedrooms, bathrooms, page, pageSize, total);

            return (items, total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error listing properties.");
            throw;
        }
    }
}
