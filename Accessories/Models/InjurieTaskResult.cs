namespace Accessories.Models;

public class InjurieTaskResult
{
    public long Id { get; set; }
    public long InjurieResultId { get; set; }
    public long TaskId { get; set; }
    public int DeductedPoints { get; set; }
    public DateTime Created { get; init; }
}
