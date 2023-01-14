namespace Accessories.Models;

public class InjurieResult
{
    public long Id { get; set; }
    public long TeamId { get; set; }
    public long InjurieId { get; set; }
    public long RefereeId { get; set; }
    public string? RefereeSignature { get; set; }
    public string? LeaderSignature { get; set; }
    public DateTime Signed { get; set; }
    public DateTime Creted { get; set; }
}
