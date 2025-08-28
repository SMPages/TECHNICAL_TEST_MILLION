using System;
using System.Collections.Generic;

namespace RealEstate.Infrastructure.Persistence.EF.Models;

public partial class Property
{
    public int IdProperty { get; set; }

    public string CodeInternal { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public decimal Price { get; set; }

    public short? Year { get; set; }

    public byte? Bedrooms { get; set; }

    public byte? Bathrooms { get; set; }

    public double? AreaSqFt { get; set; }

    public int IdOwner { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Owner IdOwnerNavigation { get; set; } = null!;

    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public virtual ICollection<PropertyTrace> PropertyTraces { get; set; } = new List<PropertyTrace>();
}
