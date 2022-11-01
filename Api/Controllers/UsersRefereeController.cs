using System.Data.Common;

using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/users/referee")]
    [ApiController]
    public class UsersRefereeController : ControllerBase
    {
        [HttpGet()]
        public async Task<List<User>> Get()
        {
            Security.Authorize(HttpContext);

            //var filters = Convert.ToString(HttpContext.Request.Form.FirstOrDefault(x => x.Key == "filters").Value);
            var users = new List<User>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `email`, `phoneNumber`, `administrator`, `signature`, `lastLogin`, `registered` FROM `user`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt64(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.GetString(4),

                    Administrator = reader.GetBoolean(5),
                    Signature = reader.GetString(6),

                    LastLogin = reader.GetDateTime(7),
                    Registered = reader.GetDateTime(8),
                });
            }
            return users;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `email`, `phoneNumber`, `administrator`, `signature`, `lastLogin`, `registered` FROM `user` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new User
                {
                    Id = reader.GetInt64(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.GetString(4),

                    Administrator = reader.GetBoolean(5),
                    Signature = reader.GetString(6),

                    LastLogin = reader.GetDateTime(7),
                    Registered = reader.GetDateTime(8),
                });
            }
            else
                return NotFound();
        }

        //POST je nahrazeno registrací v Authentikaci

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;
            
            User newUser = new()
            {
                Id = id,
                FirstName = body["firstName"],
                LastName = body["lastName"],
                Email = body["email"],
                PhoneNumber = body["phoneNumber"],
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `user` WHERE id={id}";
            using (MySqlCommand command = new(sql, connection))
            {
                var count = Convert.ToInt64(await command.ExecuteScalarAsync());
                if (count == 0)
                {
                    return NotFound();
                }
                else if (count > 1)
                {
                    return Conflict();
                }
            }

            sql = $"UPDATE `user` SET `firstName`='{newUser.FirstName}',`lastName`='{newUser.LastName}',`email`='{newUser.Email}',`phoneNumber`='{newUser.PhoneNumber}' WHERE id={id}";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteNonQueryAsync();
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT * FROM `user` WHERE id={id}";
            using (MySqlCommand command = new(sql, connection))
            {
                var count = Convert.ToInt64(await command.ExecuteScalarAsync());
                if (count == 0)
                {
                    return NotFound();
                }
                else if(count > 1)
                {
                    return Conflict();
                }
            }

            sql = $"DELETE FROM `user` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
