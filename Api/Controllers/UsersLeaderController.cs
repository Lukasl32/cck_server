using Accessories.Models;
using System.Data.Common;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/users/leader")]
    [ApiController]
    public class UsersLeaderController : ControllerBase
    {
        [HttpGet()]
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

        [HttpGet("{id}")]
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

        [HttpPost()]   //není hotové (chce vylepšit)
        public async Task<IActionResult> PostLeader()
        {
            Security.Authorize(HttpContext); //registrace podminena prihlasenim

            var body = HttpContext.Request.Form;

            User user = new()
            {
                FirstName = body["fistName"],
                LastName = body["lastName"],
                PhoneNumber = body["phoneNumber"],

                Signature = Security.GenerateHash(128)
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `users_leader`(`firstName`, `lastName`, `phoneNumber`, `signature`) " +
                    $"VALUES ('{user.FirstName}', '{user.LastName}', '{user.PhoneNumber}', '{user.Signature}');";
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
