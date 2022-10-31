using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet("/referee")]
        public async Task<List<User>> GetReferee()
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

        // GET api/<UsersController>/5
        [HttpGet("/referee/{id}")]
        public async Task<IActionResult> GetReferee(int id)
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

        // PUT api/<UsersController>/5
        [HttpPut("/referee/{id}")]
        public async Task<IActionResult> PutReferee(int id)
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

        // DELETE api/<UsersController>/5
        [HttpDelete("/referee/{id}")]
        public async Task<IActionResult> DeleteReferee(int id)
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

        [HttpGet("/leader")]
        public async Task<List<UserTeamLeader>> GetLeader()
        {
            Security.Authorize(HttpContext);

            var users = new List<UserTeamLeader>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `phoneNumber`, `signature` FROM `users_leader`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new UserTeamLeader
                {
                    Id = reader.GetInt64(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    Signature = reader.GetString(4),
                });
            }
            return users;
        }

        [HttpPost("/leader")]
        public async Task<IActionResult> PostLeader()
        {

        }

        [HttpGet("/leader/{id}")]
        public async Task<IActionResult> GetLeader(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `phoneNumber`, `signature` FROM `users_leader` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new UserTeamLeader
                {
                    Id = reader.GetInt64(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    Signature = reader.GetString(4),
                });
            }
            else
                return NotFound();
        }

        [HttpPut("/leader/{id}")]
        public async Task<IActionResult> PutLeader(int id)
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;

            UserTeamLeader newUser = new()
            {
                Id = id,
                FirstName = body["firstName"],
                LastName = body["lastName"],
                PhoneNumber = body["phoneNumber"],
                Signature = body["signature"]
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `users_leader` WHERE id={id}";
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

            sql = $"UPDATE `users_leader` SET `firstName`='{newUser.FirstName}',`lastName`='{newUser.LastName}', `phoneNumber`='{newUser.PhoneNumber}', `signature`='{newUser.Signature}' WHERE id={id}";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteNonQueryAsync();
                return Ok();
            }
        }

        [HttpDelete("/leader/{id}")]
        public async Task<IActionResult> DeleteLeader(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT * FROM `users_leader` WHERE id={id}";
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

            sql = $"DELETE FROM `users_leader` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
