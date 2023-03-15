using System.Data.Common;

using Accessories.Enums;
using Accessories.Extensions;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;


namespace Api.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Team>> Get()
        {
            Security.Authorize(HttpContext);

            var teams = new List<Team>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `number`, `organization`, `points`, `competetion_id`, `tier`+0 FROM `teams`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                teams.Add(new Team
                {
                    Id = reader.GetInt64(0),
                    Number = reader.GetByte(1),
                    Organization = reader.GetString(2),
                    Points = reader.IsDBNull(3) ? null : reader.GetDouble(3),
                    CompetitionId = reader.GetInt64(4),
                    Tier = (Tier)reader.GetInt32(5)
                });
            }
            return teams;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `number`, `organization`, `points`, `competetion_id`, `tier`+0 FROM `teams` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new Team
                {
                    Id = reader.GetInt64(0),
                    Number = reader.GetByte(1),
                    Organization = reader.GetString(2),
                    Points = reader.IsDBNull(3) ? null : reader.GetDouble(3),
                    CompetitionId = reader.GetInt64(4),
                    Tier = (Tier)reader.GetInt32(5)
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

            Team team = new()
            {
                Number = Convert.ToByte(body["number"]),
                Organization = body["organization"],
                CompetitionId = Convert.ToInt64(body["competitionId"]),
                Tier = (Tier)Convert.ToInt32(body["tier"])
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `teams`(`number`, `organization`, `competetion_id`, `tier`) " +
                    $"VALUES ('{team.Number}', {Sql.Nullable(team.Organization)}, '{team.CompetitionId}','{team.Tier}');";
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

            Team team = new()
            {
                Id = id,
                Number = Convert.ToByte(body["number"]),
                Organization = body["organization"],
                Tier = (Tier)Convert.ToInt32(body["tier"])
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `teams` WHERE id={id}";
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

            sql = $"UPDATE `teams` SET `number`='{team.Number}',`organization`='{team.Organization}', `points`={Sql.Nullable(team.Points)},`competetion_id`='{team.CompetitionId}','tier'='{team.Tier}' WHERE id={id};";
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
            sql = $"SELECT COUNT(*) FROM `teams` WHERE id={id}";
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

            sql = $"DELETE FROM `teams` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
