using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Api.DTOs;
using RealEstate.Application.DTOs;
using RealEstate.Application.Ports;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstate.Api.Controllers;

/// <summary>Endpoints para gestionar propiedades.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PropertiesController : ControllerBase
{
    private readonly ILogger<PropertiesController> _logger;
    private readonly IPropertiesService _service;

    public PropertiesController(ILogger<PropertiesController> logger, IPropertiesService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>Crea una propiedad.</summary>
    [HttpPost("CreateProperty")]
    [SwaggerOperation(
        Summary = "Create Property",
        Description = "Crea una nueva propiedad con datos básicos (código, nombre, dirección, precio, propietario).")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req, CancellationToken ct)
    {
        try
        {
            var id = await _service.CreateAsync(new CreatePropertyDto
            {
                Code = req.Code,
                Name = req.Name,
                Address = req.Address,
                City = req.City,
                Price = req.Price,
                IdOwner = req.IdOwner,
                Year = req.Year,
                Bedrooms = req.Bedrooms,
                Bathrooms = req.Bathrooms,
                AreaSqFt = req.AreaSqFt,
                Description = req.Description
            }, ct);

            _logger.LogInformation("Property created: {Code} (Id {Id})", req.Code, id);
            return CreatedAtAction(nameof(List), new { code = req.Code },
                new CreatePropertyResponse { Id = id, Code = req.Code });
        }
        catch (InvalidOperationException ex) { _logger.LogWarning(ex, "Conflict creating property"); return Conflict(new { error = ex.Message }); }
        catch (ArgumentException ex) { _logger.LogWarning(ex, "Bad request creating property"); return BadRequest(new { error = ex.Message }); }
        catch (Exception ex) { _logger.LogError(ex, "Unexpected error creating property"); return Problem("Unexpected error creating property."); }
    }

    /// <summary>Agrega una imagen a la propiedad.</summary>
    [HttpPost("{id:int}/AddImageToProperty")]
    [SwaggerOperation(
        Summary = "Add Image to Property",
        Description = "Adjunta una imagen a la propiedad. Permite marcarla como principal y definir el orden.")]
    public async Task<IActionResult> AddImage(int id, [FromBody] AddImageRequest req, CancellationToken ct)
    {
        try
        {
            var imageId = await _service.AddImageAsync(id, new AddImageDto
            {
                FileUrl = req.FileUrl,
                IsMain = req.IsMain,
                Caption = req.Caption,
                SortOrder = req.SortOrder
            }, ct);

            _logger.LogInformation("Image {ImageId} added to Property {PropertyId}", imageId, id);
            return Ok(new { imageId });
        }
        catch (KeyNotFoundException ex) { _logger.LogWarning(ex, "Property not found"); return NotFound(new { error = ex.Message }); }
        catch (ArgumentException ex) { _logger.LogWarning(ex, "Bad request adding image"); return BadRequest(new { error = ex.Message }); }
        catch (Exception ex) { _logger.LogError(ex, "Unexpected error adding image"); return Problem("Unexpected error adding image."); }
    }

    /// <summary>Cambia el precio de la propiedad.</summary>
    [HttpPut("ChangePrice/{id:int}")]
    [SwaggerOperation(
        Summary = "Change Price",
        Description = "Actualiza el precio de la propiedad y registra la traza PRICE_CHANGE.")]
    public async Task<IActionResult> ChangePrice(int id, [FromBody] ChangePriceRequest req, CancellationToken ct)
    {
        try
        {
            await _service.ChangePriceAsync(id, new ChangePriceDto { NewPrice = req.NewPrice, Notes = req.Notes }, ct);
            _logger.LogInformation("Price changed. Property {Id} -> {Price}", id, req.NewPrice);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { _logger.LogWarning(ex, "Property not found"); return NotFound(new { error = ex.Message }); }
        catch (ArgumentOutOfRangeException ex) { _logger.LogWarning(ex, "Invalid price"); return BadRequest(new { error = ex.Message }); }
        catch (Exception ex) { _logger.LogError(ex, "Unexpected error changing price"); return Problem("Unexpected error changing price."); }
    }

    /// <summary>Actualiza datos básicos de la propiedad.</summary>
    [HttpPut("UpdateProperty/{id:int}")]
    [SwaggerOperation(
        Summary = "Update Property",
        Description = "Modifica nombre, dirección, ciudad, año, dormitorios, baños y área.")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyRequest req, CancellationToken ct)
    {
        try
        {
            await _service.UpdateAsync(id, new UpdatePropertyDto
            {
                Name = req.Name,
                Address = req.Address,
                City = req.City,
                Year = req.Year,
                Bedrooms = req.Bedrooms,
                Bathrooms = req.Bathrooms,
                AreaSqFt = req.AreaSqFt,
                Description = req.Description
            }, ct);

            _logger.LogInformation("Property updated: {Id}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { _logger.LogWarning(ex, "Property not found"); return NotFound(new { error = ex.Message }); }
        catch (ArgumentException ex) { _logger.LogWarning(ex, "Bad request updating property"); return BadRequest(new { error = ex.Message }); }
        catch (Exception ex) { _logger.LogError(ex, "Unexpected error updating property"); return Problem("Unexpected error updating property."); }
    }

    /// <summary>Lista propiedades con filtros y paginación.</summary>
    [HttpGet("ListProperties")]
    [SwaggerOperation(
        Summary = "List Properties",
        Description = "Devuelve un listado paginado filtrando por ciudad, rango de precio, dormitorios y baños.")]
    [AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] ListPropertiesQuery q, CancellationToken ct)
    {
        try
        {
            var page = q.Page <= 0 ? 1 : q.Page;
            var size = q.PageSize is < 1 or > 200 ? 20 : q.PageSize;

            var (items, total) = await _service.ListAsync(new ListPropertiesQueryDto
            {
                City = q.City,
                MinPrice = q.MinPrice,
                MaxPrice = q.MaxPrice,
                Bedrooms = q.Bedrooms,
                Bathrooms = q.Bathrooms,
                Page = page,
                PageSize = size
            }, ct);

            // Headers de paginación (HTTP concern)
            Response.Headers["X-Total-Count"] = total.ToString();

            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            // Preservar filtros actuales en los links (excepto page/pageSize)
            var preserved = Request.Query
                .Where(kv => !string.Equals(kv.Key, "page", StringComparison.OrdinalIgnoreCase)
                          && !string.Equals(kv.Key, "pagesize", StringComparison.OrdinalIgnoreCase))
                .SelectMany(kv => kv.Value, (kv, val) =>
                    $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(val)}");
            var preservedQs = string.Join("&", preserved);

            string MakeLink(int p)
                => $"{baseUrl}?{(string.IsNullOrEmpty(preservedQs) ? "" : preservedQs + "&")}page={p}&pageSize={size}";

            var links = new List<string>();
            if (page > 1) links.Add($"<{MakeLink(page - 1)}>; rel=\"prev\"");
            if (page < totalPages) links.Add($"<{MakeLink(page + 1)}>; rel=\"next\"");
            if (links.Count > 0) Response.Headers["Link"] = string.Join(", ", links);

            _logger.LogInformation("Listed properties: page {Page}/{TotalPages}, size {Size}, total {Total}",
                page, totalPages, size, total);

            // Devuelve DTOs de lectura (no entidades de dominio)
            return Ok(new ListResponse<PropertyListItemDto> { Total = total, Items = items });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error listing properties.");
            return Problem("Unexpected error listing properties.");
        }
    }
}
