namespace Accessories.Extensions;

public static class Sql
{
    public static string? Nullable<T>(T value)
    {
        if (value is null)
            return "NULL";
        else if (value.GetType() == typeof(DateTime))
            return $"'{value:yyyy-MM-dd HH:mm:ss}'";
        else
            return $"'{value}'";
    }
}
