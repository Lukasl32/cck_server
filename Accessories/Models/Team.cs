namespace Accessories.Models;

public class Team
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public byte Number { get; set; }
    public string? Organization { get; set; }
    public long LeaderId { get; set; }
    public long EscortId { get; set; }
    public byte MemberCount { get; set; }
    public double Points { get; set; }
    public long CompetitionId { get; set; }
}
