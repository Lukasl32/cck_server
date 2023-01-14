using Accessories.Extensions;
using System.Data.Common;

using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/result/injurie/task")]
    [ApiController]
    public class InjurieTaskResultController : ControllerBase
    {
        //vybere vždy všechny odpovědi odpovídající zranění
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int injurieResultId)
        {
            Security.Authorize(HttpContext);

            var results = new List<InjurieTaskResult>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `result_injurie_id`, `task_id`, `deducted_points`, `created` FROM `result_tasks` WHERE `result_injurie_id`={injurieResultId};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new InjurieTaskResult
                {
                    Id = reader.GetInt64(0),
                    InjurieResultId = reader.GetInt64(1),
                    TaskId = reader.GetInt64(2),
                    DeductedPoints = reader.GetInt32(3),
                    Created = reader.GetDateTime(4)
                });
            }
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;

            InjurieTaskResult injurie = new()
            {
                InjurieResultId = Convert.ToInt64(body["injurieResultId"]),
                TaskId = Convert.ToInt64(body["taskId"]),
                DeductedPoints = Convert.ToInt32(body["deductedPoints"])
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `result_tasks`(`result_injurie_id`, `task_id`, `deducted_points`) " +
                    $"VALUES ('{injurie.InjurieResultId}', '{injurie.TaskId}', '{injurie.DeductedPoints}');";
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
