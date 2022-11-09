namespace Accessories.Models;

public class Token
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string? Hash { get; set; }
    public DateTime Created { get; set; }
    public TimeSpan Expiration { get; set; }
}
