namespace Accessories.Models;

public class Token
{
    public long UserId { get; set; }
    public string? Hash { get; set; }
    public DateTime Created { get; set; }
    public TimeSpan Expiration { get; set; }
}
