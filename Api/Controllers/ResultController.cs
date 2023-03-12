using System.Data.Common;
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
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    Security.Authorize(HttpContext);
        //    using MySqlConnection connection = new(Config.ConnString);
        //    await connection.OpenAsync();


        //    string sql = $";";
        //    using MySqlCommand command = new(sql, connection);
        //    using MySqlDataReader reader = await command.ExecuteReaderAsync();
        //    if (await reader.ReadAsync())
        //    {
                
        //    }
        //    else
        //        return NotFound();
        //}

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
            //GeneralResult data = JsonSerializer.Deserialize<GeneralResult>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true})!;
            //----------------------------------------------------------------------------------------------

            GeneralResult data = JsonSerializer.Deserialize<GeneralResult>(body["data"]!, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true})!;
            
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
