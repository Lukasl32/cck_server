using System.Data.Common;

using Accessories.Enums;
using Accessories.Extensions;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

namespace Api.Controllers
{
    [Route("api/stations")]
    [ApiController]
    public class StationController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Station>> Get()
        {
            Security.Authorize(HttpContext);

            var stations = new List<Station>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $";";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                stations.Add(new Station
                {
                    Id = reader.GetInt64(0),
                    CompetitionId = reader.GetInt64(1),
                    Title = reader.GetString(2),
                    Number = reader.GetByte(3),
                    Type = (StationType)reader.GetInt32(4),
                    Tier = (StationTier)reader.GetInt32(5),
                    Created = reader.GetDateTime(6)
                });
            }
            return stations;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $";";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new Station
                {
                    Id = reader.GetInt64(0),
                    CompetitionId = reader.GetInt64(1),
                    Title = reader.GetString(2),
                    Number = reader.GetByte(3),
                    Type = (StationType)reader.GetInt32(4),
                    Tier = (StationTier)reader.GetInt32(5),
                    Created = reader.GetDateTime(6)
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
                CompetitionId = Convert.ToInt16(body["competitionId"]),

            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `teams`(`number`, `organization`, `competetion_id`) " +
                    $"VALUES ('{team.Number}', {Sql.Nullable(team.Organization)}, '{team.CompetitionId}');";
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

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

        }
    }
}
