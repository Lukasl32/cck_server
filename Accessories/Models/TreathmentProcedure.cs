namespace Accessories.Models;

public class TreathmentProcedure
{
    public long Id { get; set; }
    public long InjurieId { get; set; }
    public string? Activity { get; set; }
    public int Order { get; set; }
}
