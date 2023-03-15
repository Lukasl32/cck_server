using Accessories.Enums;

namespace Accessories.Models;

public class Team
{
    public long Id { get; set; }
    public byte Number { get; set; }
    public string? Organization { get; set; }
    public Tier? Tier { get; set; }
    public double? Points { get; set; }
    public long CompetitionId { get; set; }
}
