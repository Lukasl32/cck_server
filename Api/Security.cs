using Accessories.Exceptions;
using Accessories.Models;

using MySqlConnector;

namespace Api;

public class Security
{
    public static string GenerateHash(uint length = 128)
    {
        var allChar = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";     //znakový prostor pro generátor
        var random = new Random();
        var resultToken = new string(
           Enumerable.Repeat(allChar, Convert.ToInt32(length))
           .Select(token => token[random.Next(token.Length)]).ToArray());
        return resultToken;
    }

    public static void Authorize(HttpContext context, bool administrator = false) => Authorize(context, out _, administrator);
    public static void Authorize(HttpContext context, out Token token, bool administrator = false)
    {
        string? tokenHash = context.Request.Headers["token"];
        if (tokenHash == null)
            throw new AuthorizationException();

        using (MySqlConnection connection = new(Config.ConnString))
        {
            connection.Open();
            string sql;

            sql = $"SELECT `id`, `user_id`, `token`, `created`, `expiration` FROM `tokens` WHERE token='{tokenHash}';";
            using (MySqlCommand command = new(sql, connection))
            {
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    token = new()
                    {
                        Id = reader.GetInt64(0),
                        UserId = reader.GetInt64(1),
                        Hash = reader.GetString(2),
                        Created = reader.GetDateTime(3),
                        Expiration = TimeSpan.FromSeconds(reader.GetInt64(4)),
                    };

                    if ((DateTime.Now - token.Created).TotalSeconds >= token.Expiration.TotalSeconds)
                        throw new AuthorizationException();
                }
                else
                    throw new AuthorizationException();
            }

            //administrator check
            if (administrator)
            {
                sql = $"SELECT `administrator` FROM `user` WHERE id={token.UserId};";
                using MySqlCommand command = new(sql, connection);
                if (!Convert.ToBoolean(command.ExecuteScalar()))
                    throw new AuthorizationException();
            }
        }
    }

    public static async Task SaveToken(Token token)
    {
        using MySqlConnection connection = new(Config.ConnString);
        await connection.OpenAsync();
        string sql = $"INSERT INTO `tokens`(`user_id`, `token`, `created`, `expiration`) VALUES ('{token.UserId}', '{token.Hash}', '{token.Created:yyyy-MM-dd HH:mm:ss}', '{token.Expiration.TotalSeconds}');";
        using MySqlCommand command = new(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
