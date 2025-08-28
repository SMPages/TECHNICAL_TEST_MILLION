namespace RealEstate.Domain.Entities;

/// <summary>
/// Historial de eventos (cambios de precio, venta) - tabla dbo.PropertyTrace.
/// </summary>
public sealed class PropertyTrace
{
    public int IdPropertyTrace { get; internal set; }
    public int IdProperty { get; internal set; }
    public DateTime? DateSale { get; internal set; }     // null para PRICE_CHANGE
    public string Name { get; internal set; } = default!; // 'SALE' | 'PRICE_CHANGE'
    public decimal Value { get; internal set; }           // precio nuevo o precio de venta
    public decimal Tax { get; internal set; }             // si aplica para venta
    public DateTime CreatedAt { get; internal set; }

    public Property? Property { get; internal set; }

    private PropertyTrace() { } // EF

    public PropertyTrace(int idProperty, string name, decimal value, decimal tax = 0, DateTime? dateSale = null)
    {
        if (idProperty <= 0) throw new ArgumentOutOfRangeException(nameof(idProperty));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        IdProperty = idProperty;
        Name = name.Trim();
        Value = value;
        Tax = tax;
        DateSale = dateSale;
        CreatedAt = DateTime.UtcNow;
    }
}
