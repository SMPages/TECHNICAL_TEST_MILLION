using System;
using System.Collections.Generic;

namespace RealEstate.Infrastructure.Persistence.EF.Models;

public partial class Owner
{
    public int IdOwner { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Photo { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
