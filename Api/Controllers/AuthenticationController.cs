using System.Data.Common;

using Accessories.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;

using MySqlConnector;

namespace Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var body = HttpContext.Request.Form;
            string email = body["email"];
            string password = body["password"];

            long id;
            string? passwordHash;

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();
                string sql = $"SELECT `id`, `password` FROM `user` WHERE email='{email}';";
                using MySqlCommand command = new(sql, connection);
                using MySqlDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    id = reader.GetInt64(0);
                    passwordHash = reader.GetString(1);
                }
                else
                    return Forbid();
            }

            if(BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, BCrypt.Net.HashType.SHA512))
            {
                Token token = new()
                {
                    UserId = id,
                    Hash = Security.GenerateHash(128),
                    Created = DateTime.Now,
                    Expiration = TimeSpan.FromHours(1)
                };
                await Security.SaveToken(token);
                await LogLogin(id);
                return Ok(token);
            }
            else
                return Forbid();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            Security.Authorize(HttpContext); //registrace podminena prihlasenim

            var body = HttpContext.Request.Form;

            User user = new()
            {
                FirstName = body["fistName"],
                LastName = body["lastName"],
                Email = body["email"],
                PhoneNumber = body["phoneNumber"],

                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(body["password"], BCrypt.Net.HashType.SHA512),

                Administrator = Convert.ToBoolean(Convert.ToInt16(body["administrator"])),
                Signature = Security.GenerateHash(128)
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `user`(`firstName`, `lastName`, `email`, `phoneNumber`, `password`, `administrator`, `signature`) " +
                    $"VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{user.PhoneNumber}', '{user.Password}', '{Convert.ToInt16(user.Administrator)}', '{user.Signature}');";
                using MySqlCommand command = new(sql, connection);
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (DbException)
                {
                    //DB error code: 1062 - duplicated entry
                    return Conflict();
                }
            }

            return StatusCode(201);
        }

        private static async Task LogLogin(long userId)
        {
            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"UPDATE `user` SET `lastLogin`='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' WHERE id={userId};";
            using MySqlCommand command = new(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
