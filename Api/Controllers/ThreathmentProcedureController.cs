using Accessories.Models;
using System.Data.Common;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/threathmentprocedures")]
    [ApiController]
    public class ThreathmentProcedureController : ControllerBase
    {
        [HttpGet]
        public async Task<List<ThreathmentProcedure>> Get()
        {
            Security.Authorize(HttpContext);

            var threathmentProcedures = new List<ThreathmentProcedure>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `injurie_id`, `activity`, `order` FROM `threathment_procedures`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                threathmentProcedures.Add(new ThreathmentProcedure
                {
                    Id = reader.GetInt64(0),
                    InjurieId = reader.GetInt64(1),
                    Activity = reader.GetString(2),
                    Order= reader.GetInt32(3),
                });
            }
            return threathmentProcedures;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `injurie_id`, `activity`, `order` FROM `threathment_procedures` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new ThreathmentProcedure
                {
                    Id = reader.GetInt64(0),
                    InjurieId = reader.GetInt64(1),
                    Activity = reader.GetString(2),
                    Order = reader.GetInt32(3),
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

            ThreathmentProcedure threathmentProcedure = new()
            {
                InjurieId = Convert.ToInt64(body["injurieId"]),
                Activity = body["activity"],
                Order = Convert.ToInt32(body["order"])
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `threathment_procedures`(`injurie_id`, `activity`, `order`)" +
                    $" VALUES ('{threathmentProcedure.InjurieId}', '{threathmentProcedure.Activity}', '{threathmentProcedure.Order}');";
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

            ThreathmentProcedure threathmentProcedure = new()
            {
                InjurieId = Convert.ToInt64(body["injurieId"]),
                Activity = body["activity"],
                Order = Convert.ToInt32(body["order"])
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `threathment_procedures` WHERE id={id}";
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

            sql = $"UPDATE `threathment_procedures` SET `injurie_id`='{threathmentProcedure.InjurieId}',`activity`='{threathmentProcedure.Activity}',`order`='{threathmentProcedure.Order}' WHERE id={id};";
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
            sql = $"SELECT COUNT(*) FROM `threathment_procedures` WHERE id={id}";
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

            sql = $"DELETE FROM `threathment_procedures` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
