namespace RealEstate.Domain.Entities;

/// <summary>
/// Inmueble (tabla dbo.Property).
/// </summary>
public sealed class Property
{
    public int IdProperty { get; internal set; }
    public string CodeInternal { get; internal set; } = default!;
    public string Name { get; internal set; } = default!;
    public string Address { get; internal set; } = default!;
    public string? City { get; internal set; }
    public decimal Price { get; internal set; }
    public short? Year { get; internal set; }           // Mapea a columna [Year]
    public byte? Bedrooms { get; internal set; }
    public byte? Bathrooms { get; internal set; }
    public double? AreaSqFt { get; internal set; }
    public int IdOwner { get; internal set; }
    public DateTime CreatedAt { get; internal set; }

    // Navegación (no obligatoria para dominio, pero útil)
    public Owner? Owner { get; private set; }
    public ICollection<PropertyImage> Images { get; private set; } = new List<PropertyImage>();
    public ICollection<PropertyTrace> Traces { get; private set; } = new List<PropertyTrace>();

    private Property() { } // EF

    public Property(string codeInternal, string name, string address, int idOwner, decimal price)
    {
        if (string.IsNullOrWhiteSpace(codeInternal)) throw new ArgumentException("CodeInternal is required.", nameof(codeInternal));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address is required.", nameof(address));
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than zero.");
        if (idOwner <= 0) throw new ArgumentOutOfRangeException(nameof(idOwner));

        CodeInternal = codeInternal.Trim();
        Name = name.Trim();
        Address = address.Trim();
        IdOwner = idOwner;
        Price = price;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Cambia el precio del inmueble. Se recomienda registrar un PropertyTrace en la capa de aplicación.</summary>
    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0) throw new ArgumentOutOfRangeException(nameof(newPrice), "New price must be greater than zero.");
        Price = newPrice;
    }

    /// <summary>Actualiza datos básicos y filtros comunes.</summary>
    public void UpdateBasicInfo(
     string name,
     string address,
     string? city,
     short? year,
     byte? bedrooms,
     byte? bathrooms,
     double? areaSqFt)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
        if (!string.IsNullOrWhiteSpace(address)) Address = address.Trim();
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        Year = year;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        AreaSqFt = areaSqFt;
    }

    /// <summary>Agrega una imagen a la colección local (persistencia se hace en repos).</summary>
    public void AddImage(string fileUrl, bool isMain = false, string? caption = null, int sortOrder = 0)
    {
        var img = new PropertyImage(IdProperty, fileUrl, isMain, caption, sortOrder);
        Images.Add(img);
    }
}
