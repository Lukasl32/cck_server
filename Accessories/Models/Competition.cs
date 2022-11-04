using Accessories.Enums;

namespace Accessories.Models;

public class Competition
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CompetitionType Type { get; set; }
    public string? Description { get; set; }
}
