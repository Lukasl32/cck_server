namespace Accessories;

public static class Sql
{
    public static string? Nullable<T>(T value)
    {
        if (value is null)
            return "NULL";
        else
            return $"'{value}'";
    }
}
