using System.Data.Common;

using Accessories.Enums;
using Accessories.Extensions;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/injuries")]
    [ApiController]
    public class InjuriesController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Injurie>> Get()
        {
            Security.Authorize(HttpContext);

            var injurie = new List<Injurie>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `station_id`, `referee_id`, `letter`, `situation`, `diagnosis`, `maximalPoints`, `necessaryEquipment`, `info` FROM `injuries`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                injurie.Add(new Injurie
                {
                    Id = reader.GetInt64(0),
                    StationId = reader.GetInt64(1),
                    RefereeId = reader.GetInt64(2),
                    Letter = reader.GetChar(3),
                    Situation = reader.GetString(4),
                    Diagnose = reader.GetString(5),
                    MaximalPoints = reader.GetInt32(6),
                    NeccesseryEquipment= reader.GetString(7),
                    Info = reader.GetString(8),
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
            string sql = $"SELECT `id`, `station_id`, `referee_id`, `letter`, `situation`, `diagnosis`, `maximalPoints`, `necessaryEquipment`, `info` FROM `injuries` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new Injurie
                {
                    Id = reader.GetInt64(0),
                    StationId = reader.GetInt64(1),
                    RefereeId = reader.GetInt64(2),
                    Letter = reader.GetChar(3),
                    Situation = reader.GetString(4),
                    Diagnose = reader.GetString(5),
                    MaximalPoints = reader.GetInt32(6),
                    NeccesseryEquipment = reader.GetString(7),
                    Info = reader.GetString(8),
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

            Injurie injurie = new()
            {
                StationId = Convert.ToInt64(body["stationId"]),
                RefereeId = Convert.ToInt64(body["refereeId"]),
                Letter = Convert.ToChar(body["letter"]),
                Situation = body["situation"],
                Diagnose = body["diagnose"],
                MaximalPoints = Convert.ToInt32(body["maximalPoints"]),
                NeccesseryEquipment = body["neccesseryEquipment"],
                Info = body["info"]
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `injuries`(`station_id`, `referee_id`, `letter`, `situation`, `diagnosis`, `maximalPoints`, `necessaryEquipment`, `info`) " +
                    $"VALUES ('{injurie.StationId}', '{injurie.RefereeId}', '{injurie.Letter}', '{injurie.Situation}', '{injurie.Diagnose}', '{injurie.MaximalPoints}', {Sql.Nullable(injurie.NeccesseryEquipment)}, {Sql.Nullable(injurie.Info)});";
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

            Injurie injurie = new()
            {
                StationId = Convert.ToInt64(body["stationId"]),
                RefereeId = Convert.ToInt64(body["refereeId"]),
                Letter = Convert.ToChar(body["letter"]),
                Situation = body["situation"],
                Diagnose = body["diagnose"],
                MaximalPoints = Convert.ToInt32(body["maximalPoints"]),
                NeccesseryEquipment = body["neccesseryEquipment"],
                Info = body["info"]
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

            sql = $"UPDATE `injuries` SET `station_id`='{injurie.StationId}',`referee_id`='{injurie.RefereeId}',`letter`='{injurie.Letter}',`situation`='{injurie.Situation}',`diagnosis`='{injurie.Diagnose}',`maximalPoints`='{injurie.MaximalPoints}',`necessaryEquipment`={Sql.Nullable(injurie.NeccesseryEquipment)},`info`={Sql.Nullable(injurie.Info)} WHERE id={id};";
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

            sql = $"DELETE FROM `injuries` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
