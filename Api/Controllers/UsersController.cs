using System.IO;
using System.Net.Mail;

using Accessories.Enums;
using Accessories.Exceptions;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet()]
        public async Task<List<User>> Get()
        {
            //var filters = Convert.ToString(HttpContext.Request.Form.FirstOrDefault(x => x.Key == "filters").Value);
            var users = new List<User>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `email`, `phoneNumber`, `administration`, `signature`, `lastLogin`, `registered` FROM `user`;";
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

                    Administration = reader.GetBoolean(5),
                    Signature = reader.GetString(6),

                    LastLogin = reader.GetDateTime(7),
                    Registered = reader.GetDateTime(8),
                });
            }
            return users;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `email`, `phoneNumber`, `administration`, `signature`, `lastLogin`, `registered` FROM `user`;";
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

                    Administration = reader.GetBoolean(5),
                    Signature = reader.GetString(6),

                    LastLogin = reader.GetDateTime(7),
                    Registered = reader.GetDateTime(8),
                });
            }
            else
                return NotFound();
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
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
            sql = $"SELECT * FROM `user` WHERE id={id}";
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
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
