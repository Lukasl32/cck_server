using System.Data;
using System.Data.Common;
using Accessories.Enums;
using Accessories.Extensions;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Dependency;

using MySqlConnector;

namespace Api.Controllers
{
    [Route("api/competitions")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Competition>> Get()
        {
            Security.Authorize(HttpContext);

            var competitions = new List<Competition>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `startDate`, `endDate`, `type`+0, `description` FROM `competitions`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                competitions.Add(new Competition
                {
                    Id = reader.GetInt64(0),
                    StartDate = reader.GetDateTime(1),
                    EndDate = reader.GetDateTime(2),
                    Type = (CompetitionType)reader.GetInt32(3),
                    Description = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
            return competitions;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `startDate`, `endDate`, `type`+0, `description` FROM `competitions` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new Competition
                {
                    Id = reader.GetInt64(0),
                    StartDate = reader.GetDateTime(1),
                    EndDate = reader.GetDateTime(2),
                    Type = (CompetitionType)reader.GetInt32(3),
                    Description = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Security.Authorize(HttpContext); //registrace podminena prihlasenim

            var body = HttpContext.Request.Form;

            Competition competition = new()
            {
                StartDate = Convert.ToDateTime(body["startDate"]),
                EndDate = Convert.ToDateTime(body["endDate"]),
                Type = (CompetitionType)Convert.ToInt32(body["type"]),
                Description = body["description"]
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `competitions`(`startDate`, `endDate`, `type`, `description`) " +
                    $"VALUES ('{competition.StartDate:yyyy-MM-dd}', '{competition.EndDate:yyyy-MM-dd}', '{(int)competition.Type}', {Sql.Nullable(competition.Description)});";
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

            Competition competition = new()
            {
                Id = id,
                StartDate = Convert.ToDateTime(body["startDate"]),
                EndDate = Convert.ToDateTime(body["endDate"]),
                Type = (CompetitionType)Convert.ToInt32(body["type"]),
                Description = body["description"]
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `competitions` WHERE id={id}";
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

            sql = $"UPDATE `competitions` SET `startDate`='{competition.StartDate:yyyy-MM-dd}',`endDate`='{competition.EndDate:yyyy-MM-dd}',`type`='{(int)competition.Type}',`description`={Sql.Nullable(competition.Description)} WHERE id={id};";
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
            sql = $"SELECT COUNT(*) FROM `competitions` WHERE id={id}";
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

            sql = $"DELETE FROM `competitions` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
