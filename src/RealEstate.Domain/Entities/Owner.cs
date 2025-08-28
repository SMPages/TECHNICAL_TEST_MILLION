namespace RealEstate.Domain.Entities;

/// <summary>
/// Propietario del inmueble (tabla dbo.Owner).
/// </summary>
public sealed class Owner
{
    public int IdOwner { get; internal set; }
    public string Name { get; internal set; } = default!;
    public string? Address { get; internal set; }
    public string? Photo { get; internal set; }
    public DateTime? Birthday { get; internal set; }
    public string? Email { get; internal set; }
    public string? Phone { get; internal set; }
    public DateTime CreatedAt { get; internal set; }

    private Owner() { } // Requerido por EF

    public Owner(string name, string? address = null, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        Name = name.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContact(string? address, string? email, string? phone)
    {
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
    }

    public void SetPhoto(string? photo) => Photo = string.IsNullOrWhiteSpace(photo) ? null : photo.Trim();
    public void SetBirthday(DateTime? date) => Birthday = date;
}
