using Accessories.Extensions;
using Accessories.Models;
using System.Data.Common;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/figurants")]
    [ApiController]
    public class FigurantsController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Figurant>> Get()
        {
            Security.Authorize(HttpContext);

            var injurie = new List<Figurant>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `injurie_id`, `instructions`, `makeup` FROM `figurants`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                injurie.Add(new Figurant
                {
                    Id = reader.GetInt64(0),
                    InjurieId = reader.GetInt64(1),
                    Instructions= reader.GetString(2),
                    MakeUp=reader.GetString(3),
                });
            }
            return injurie;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `injurie_id`, `instructions`, `makeup` FROM `figurants` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new Figurant
                {
                    Id = reader.GetInt64(0),
                    InjurieId = reader.GetInt64(1),
                    Instructions = reader.GetString(2),
                    MakeUp = reader.GetString(3),
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

            Figurant figurant = new()
            {
                InjurieId = Convert.ToInt64(body["injurieId"]),
                Instructions = body["instructions"],
                MakeUp = body["makeUp"]
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `figurants`(`injurie_id`, `instructions`, `makeup`) " +
                    $"VALUES ('{figurant.InjurieId}', '{figurant.Instructions}', '{figurant.MakeUp}');";
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

            Figurant figurant = new()
            {
                InjurieId = Convert.ToInt64(body["injurieId"]),
                Instructions = body["instructions"],
                MakeUp = body["makeUp"]
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `injuries` WHERE id={id}";
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

            sql = $"UPDATE `figurants` SET `injurie_id`='{figurant.InjurieId}',`instructions`={Sql.Nullable(figurant.Instructions)},`makeup`={Sql.Nullable(figurant.MakeUp)} WHERE id={id};";
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
            sql = $"SELECT COUNT(*) FROM `figurants` WHERE id={id}";
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

            sql = $"DELETE FROM `figurants` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
