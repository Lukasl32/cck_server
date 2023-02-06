using Accessories.Extensions;
using Accessories.Models;
using System.Data.Common;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/injurietasks")]
    [ApiController]
    public class InjurieTaskController : ControllerBase
    {
        [HttpGet]
        public async Task<List<InjurieTask>> Get()
        {
            Security.Authorize(HttpContext);

            var injurieTask = new List<InjurieTask>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `injurie_id`, `title`, `maximalMinusPoints` FROM `tasks`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                injurieTask.Add(new InjurieTask
                {
                    Id = reader.GetInt64(0),
                    InjurieId = reader.GetInt64(1),
                    Title = reader.GetString(2),
                    MaximalMinusPoints = reader.GetInt32(3),
                });
            }
            return injurieTask;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `injurie_id`, `title`, `maximalMinusPoints` FROM `tasks` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new InjurieTask
                {
                    Id = reader.GetInt64(0),
                    InjurieId = reader.GetInt64(1),
                    Title = reader.GetString(2),
                    MaximalMinusPoints = reader.GetInt32(3),
                });
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;

            InjurieTask injurieTask = new()
            {
                InjurieId = Convert.ToInt64(body["injurieId"]),
                Title = body["title"],
                MaximalMinusPoints = Convert.ToInt32(body["maximalMinusPoints"])
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `tasks`(`injurie_id`, `title`, `maximalMinusPoints`) " +
                    $"VALUES ('{injurieTask.InjurieId}', '{injurieTask.Title}', '{injurieTask.MaximalMinusPoints}');";
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
        public async Task<IActionResult> Put(int id)
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;

            InjurieTask injurieTask = new()
            {
                InjurieId = Convert.ToInt64(body["injurieId"]),
                Title = body["title"],
                MaximalMinusPoints = Convert.ToInt32(body["maximalMinusPoints"])
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `tasks` WHERE id={id}";
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

            sql = $"UPDATE `tasks` SET `injurie_id`='{injurieTask.InjurieId}',`title`='{injurieTask.Title}',`maximalMinusPoints`='{injurieTask.MaximalMinusPoints}' WHERE id={id};";
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
            sql = $"SELECT COUNT(*) FROM `tasks` WHERE id={id}";
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

            sql = $"DELETE FROM `tasks` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
