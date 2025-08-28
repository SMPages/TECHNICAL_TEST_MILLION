using RealEstate.Application.Ports;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.UseCases;

public sealed class AddImage
{
    private readonly IPropertyRepository _repo;
    public AddImage(IPropertyRepository repo) => _repo = repo;

    public async Task<int> ExecuteAsync(int idProperty, string fileUrl, bool isMain, string? caption, int sortOrder, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync(idProperty, asNoTracking: true, ct)
                ?? throw new KeyNotFoundException("Property not found");
        var img = new PropertyImage(p.IdProperty, fileUrl, isMain, caption, sortOrder);
        await _repo.AddImageAsync(img, ct);
        return img.IdPropertyImage;
    }
}
