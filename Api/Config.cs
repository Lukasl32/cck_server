namespace Api;

public class Config
{
    public static string? ConnString;

    static IConfiguration? config;
    public static void Initialize()
    {
        config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        Load();
    }

    private static void Load()
    {
        ConnString = config.GetValue<string?>("Settings:Database:ConnectionString");
    }
}
