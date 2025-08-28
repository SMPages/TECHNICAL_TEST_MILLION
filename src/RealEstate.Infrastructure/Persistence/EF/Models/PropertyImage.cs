using System;
using System.Collections.Generic;

namespace RealEstate.Infrastructure.Persistence.EF.Models;

public partial class PropertyImage
{
    public int IdPropertyImage { get; set; }

    public int IdProperty { get; set; }

    public string FileUrl { get; set; } = null!;

    public bool Enabled { get; set; }

    public bool IsMain { get; set; }

    public string? Caption { get; set; }

    public int SortOrder { get; set; }

    public virtual Property IdPropertyNavigation { get; set; } = null!;
}
