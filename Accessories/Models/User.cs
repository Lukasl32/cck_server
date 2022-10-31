using Accessories.Enums;

namespace Accessories.Models;

public class User
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    public bool? Administrator { get; set; }
    public string? Signature { get; set; }

    public DateTime LastLogin { get; set; }
    public DateTime Registered { get; set; }
}
