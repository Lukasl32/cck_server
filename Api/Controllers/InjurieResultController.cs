using System.Data.Common;

using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/result/injurie")]
    [ApiController]
    public class InjurieResultController : ControllerBase
    {
        //načtení všech výsledků z konkrétního stanoviště (station)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Security.Authorize(HttpContext);

            var stationId = HttpContext.Request.Headers["stationId"];

            var results = new List<InjurieResult>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT result_injuries.* FROM `result_injuries` INNER JOIN injuries ON result_injuries.injurie_id = injuries.id WHERE injuries.station_id={stationId};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new InjurieResult
                {
                    Id = reader.GetInt64(0),
                    TeamId = reader.GetInt64(1),
                    InjurieId = reader.GetInt64(2),
                    RefereeId = reader.GetInt64(3),
                    RefereeSignature = reader.GetString(4),
                    LeaderSignature = reader.GetString(5),
                    Signed = reader.GetDateTime(6),
                    Creted = reader.GetDateTime(7),
                });
            }
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            var results = new List<InjurieResult>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `team_id`, `injurie_id`, `referee_id`, `referee_signature`, `leader_signature`, `signed`, `created` FROM `result_injuries` WHERE injurie_id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new InjurieResult
                {
                    Id = reader.GetInt64(0),
                    TeamId = reader.GetInt64(1),
                    InjurieId = reader.GetInt64(2),
                    RefereeId = reader.GetInt64(3),
                    RefereeSignature = reader.GetString(4),
                    LeaderSignature = reader.GetString(5),
                    Signed = reader.GetDateTime(6),
                    Creted = reader.GetDateTime(7),
                });
            }
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;

            InjurieResult injurie = new()
            {
                TeamId = Convert.ToInt64(body["teamId"]),
                InjurieId = Convert.ToInt64(body["injurieId"]),
                RefereeId = Convert.ToInt64(body["refereeId"]),
                RefereeSignature = body["refereeSignature"],
                LeaderSignature = body["leaderSignature"],
                Signed = Convert.ToDateTime(body["signed"])
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `result_injuries`(`team_id`, `injurie_id`, `referee_id`, `referee_signature`, `leader_signature`, `signed`) " +
                    $"VALUES ('{injurie.TeamId}', '{injurie.InjurieId}', '{injurie.RefereeId}', '{injurie.RefereeSignature}', '{injurie.LeaderSignature}', '{injurie.Signed:yyyy-MM-dd HH:mm:ss}');";
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
    }
}
