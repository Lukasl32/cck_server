using System.Data.Common;
using System.Text;
using System.Text.Json;

using Accessories.Enums;
using Accessories.Extensions;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/results")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        [HttpGet("finalStandings/")]
        public async Task<List<TeamResult>> GetFinalStandings(int id)
        {
            Security.Authorize(HttpContext);

            var header = HttpContext.Request.Headers;

            int competitionId = Convert.ToInt32(header["competitionId"]);
            Tier tier = (Tier)Convert.ToInt32(header["tier"]);

            List<TeamResult> result = new();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();

            var teams = new List<Team>();

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

            var toDelete = teams.ToList().FindAll(x => x.CompetitionId != id || x.Tier != tier);
            foreach (var item in toDelete)
                teams.Remove(item);

            foreach (var team in teams)
            {
                var tid = team.Id;
                sql = $"SELECT SUM(result_tasks.deducted_points) from result_tasks INNER JOIN result_injuries ON result_injuries.id=result_tasks.result_injurie_id WHERE result_injuries.team_id={tid};";
                using MySqlCommand command2 = new(sql, connection);
                int points = 0;
                try
                {
                    points = (int)await command2.ExecuteScalarAsync();
                }
                catch (Exception)
                { }
                result.Add(new()
                {
                    Points = points,
                    Number = team.Number,
                    TeamId = tid
                });
            }
            return (from entry in result orderby entry.Points descending select entry).ToList();
        }

        [HttpGet("team/{id}")]
        public async Task<dynamic> GetTeamPoints(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();

            string sql = $"SELECT SUM(result_tasks.deducted_points) from result_tasks INNER JOIN result_injuries ON result_injuries.id=result_tasks.result_injurie_id WHERE result_injuries.team_id={id};";
            using MySqlCommand command2 = new(sql, connection);
            int points = 0;
            try
            {
                points = Convert.ToInt32(await command2.ExecuteScalarAsync());
            }
            catch (Exception)
            { }
            return new
            {
                Points = points
            };
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;
            
            //----------------------------------------------------------------------------------------------
            List<ResultTask> resultTasks = new();
            for (int i = 0; i < 10; i++)
            {
                resultTasks.Add(new()
                {
                    Id = i,
                    DeductedPoints = new Random().Next(-200, 200)
                });
            }

            GeneralResult result = new()
            {
                InjurieId = 5,
                TeamId = 6,
                RefereeId = 7,
                Signature = new()
                {
                    Referee = Security.GenerateHash(128),
                    Leader = Security.GenerateHash(128),
                    Signed = DateTime.Now
                },
                ResultTasks = resultTasks.ToArray()
            };
            string json = JsonSerializer.Serialize(result);
            //----------------------------------------------
            //Pro testování
            //GeneralResult data = JsonSerializer.Deserialize<GeneralResult>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true})!;
            //----------------------------------------------------------------------------------------------

            //Pro provozní nasazení
            GeneralResult data = JsonSerializer.Deserialize<GeneralResult>(Encoding.UTF8.GetString(Convert.FromBase64String(body["data"]!)), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true})!;
            
            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                long resultInjurieId;

                string sql = "INSERT INTO `result_injuries`(`team_id`, `injurie_id`, `referee_id`, `referee_signature`, `leader_signature`, `signed`) " +
                    $"VALUES ('{data.TeamId}','{data.InjurieId}','{data.RefereeId}','{data.Signature.Referee}','{data.Signature.Leader}','{data.Signature.Signed:yyyy-MM-dd HH:mm:ss}');" +
                    $"SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new(sql, connection))
                {
                    try
                    {
                        resultInjurieId = Convert.ToInt64(await command.ExecuteScalarAsync());
                    }
                    catch (DbException ex)
                    {
                        //DB error code: 1062 - duplicated entry
                        return Conflict();
                    }
                }

                foreach (var task in data.ResultTasks)
                {
                    sql = "INSERT INTO `result_tasks`(`result_injurie_id`, `task_id`, `deducted_points`) " +
                        $"VALUES ('{resultInjurieId}','{task.Id}','{task.DeductedPoints}');";
                    using (MySqlCommand command = new(sql, connection))
                    {
                        try
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        catch (DbException ex)
                        {
                            //DB error code: 1062 - duplicated entry
                            return Conflict();
                        }
                    }
                }

            }
            return StatusCode(201);
        }
    }
}
