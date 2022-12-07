namespace Accessories.Models;

public class Injurie
{
    public long Id { get; set; }
    public long StationId { get; set; }
    public long RefereeId { get; set; }
    public char Letter { get; set; }
    public string? Situation { get; set; }
    public string? Diagnose { get; set; }
    public int MaximalPoints { get; set; }
    public string? NeccesseryEquipment { get; set; }
    public string? Info { get; set; }
}
