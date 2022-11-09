using Accessories.Enums;

namespace Accessories.Models;

public class Station
{
    public long Id { get; set; }
    public long CompetitionId { get; set; }
    public string? Title { get; set; }
    public int Number { get; set; }
    public StationType Type { get; set; }
    public StationTier Tier { get; set; }
    public DateTime? Created { get; set; }
}
