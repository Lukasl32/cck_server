namespace Accessories.Models;

public class InjurieTask
{
    public long Id { get; set; }
    public long InjurieId { get; set; }
    public string? Title { get; set; }
    public int MaximalMinusPoints { get; set; }
}
