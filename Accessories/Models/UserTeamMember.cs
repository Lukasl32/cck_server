using Accessories.Enums;

namespace Accessories.Models;

public class UserTeamMember
{
    public long Id { get; set; }
    public long TeamId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public TeamMemberType Type { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birthdate { get; set; }
}
