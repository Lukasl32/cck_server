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
            string sql = $"SELECT `id`, `competetion_id`, `title`, `number`, `type`+0, `tier`+0, `created` FROM `stations`;";
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
            string sql = $"SELECT `id`, `competetion_id`, `title`, `number`, `type`+0, `tier`+0, `created` FROM `stations` WHERE id={id};";
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

            Station station = new()
            {
                CompetitionId = Convert.ToInt16(body["competitionId"]),
                Title = body["title"],
                Number = Convert.ToInt32(body["number"]),
                Type = (StationType)Convert.ToInt32(body["type"]),
                Tier = (StationTier)Convert.ToInt32(body["tier"]),
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `stations`(`competetion_id`, `title`, `number`, `type`, `tier`) " +
                    $"VALUES ('{station.CompetitionId}', '{station.Title}', '{station.Number}', '{(int)station.Type}', '{(int)station.Tier}');";
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

            Station station = new()
            {
                Id = id,
                CompetitionId = Convert.ToInt64(body["competitionId"]),
                Title = body["title"],
                Number = Convert.ToByte(body["number"]),
                Type = (StationType)Convert.ToInt32(body["type"]),
                Tier = (StationTier)Convert.ToInt32(body["tier"])
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `stations` WHERE id={id}";
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

            sql = $"UPDATE `stations` SET `competetion_id`='{station.CompetitionId}',`title`='{station.Title}',`number`='{station.Number}',`type`='{(int)station.Type}',`tier`='{(int)station.Tier}' WHERE id={id};";
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
            sql = $"SELECT * FROM `stations` WHERE id={id}";
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

            sql = $"DELETE FROM `stations` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
