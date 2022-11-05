using MySqlConnector;

namespace Accessories.Extensions;

public static class MySqlDataReaderExtensions
{
    public static DateTime? GetNullableDateTime(this MySqlDataReader reader, int ordial)
    {
        return reader.IsDBNull(ordial) ? null : reader.GetDateTime(ordial);
    }
}
