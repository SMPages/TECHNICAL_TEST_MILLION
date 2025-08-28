namespace RealEstate.Domain.Entities;

/// <summary>
/// Imagen de propiedad (tabla dbo.PropertyImage).
/// </summary>
public sealed class PropertyImage
{
    public int IdPropertyImage { get; internal set; }
    public int IdProperty { get; internal set; }
    public string FileUrl { get; internal set; } = default!;
    public bool Enabled { get; internal set; } = true;
    public bool IsMain { get; internal set; }
    public string? Caption { get; internal set; }
    public int SortOrder { get; internal set; }

    public Property? Property { get; private set; }

    private PropertyImage() { } // EF

    public PropertyImage(int idProperty, string fileUrl, bool isMain = false, string? caption = null, int sortOrder = 0)
    {
        if (idProperty <= 0) throw new ArgumentOutOfRangeException(nameof(idProperty));
        if (string.IsNullOrWhiteSpace(fileUrl)) throw new ArgumentException("FileUrl is required.", nameof(fileUrl));

        IdProperty = idProperty;
        FileUrl = fileUrl.Trim();
        IsMain = isMain;
        Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();
        SortOrder = sortOrder;
    }

    public void SetMain(bool isMain) => IsMain = isMain;
    public void Enable() => Enabled = true;
    public void Disable() => Enabled = false;
    public void UpdateCaption(string? caption) => Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();
    public void UpdateSortOrder(int sortOrder) => SortOrder = sortOrder;
}
