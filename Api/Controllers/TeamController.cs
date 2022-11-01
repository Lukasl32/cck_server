using System.Data.Common;

using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;


namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        // GET: api/<TeamController>
        [HttpGet]
        public async Task<List<Team>> Get()
        {
            Security.Authorize(HttpContext);

            var teams = new List<Team>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `firstName`, `lastName`, `phoneNumber`, `signature` FROM `users_leader`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                teams.Add(new Team
                {
                    Id = reader.GetInt64(0),
                    Title = reader.GetString(1),
                    Number = reader.GetByte(2),
                    Organization = reader.GetString(3),
                    LeaderId = reader.GetInt64(4),
                    EscortId = reader.GetInt64(5),
                    MemberCount = reader.GetByte(6),
                    Points = reader.GetDouble(7),
                    CompetitionId = reader.GetInt64(8),
                });
            }
            return teams;
        }

        // GET api/<TeamController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `title`, `number`, `organization`, `leader_id`, `escort_id`, `memberCount`, `points`, `competetion_id` FROM `teams`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new Team
                {
                    Id = reader.GetInt64(0),
                    Title = reader.GetString(1),
                    Number = reader.GetByte(2),
                    Organization = reader.GetString(3),
                    LeaderId = reader.GetInt64(4),
                    EscortId = reader.GetInt64(5),
                    MemberCount = reader.GetByte(6),
                    Points = reader.GetDouble(7),
                    CompetitionId = reader.GetInt64(8),
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

            Team team = new()
            {
                Title = body["title"],
                Number = Convert.ToByte(body["number"]),
                Organization = body["organization"],
                LeaderId = Convert.ToInt64(body["leaderId"]),
                EscortId = Convert.ToInt64(body["escortId"]),
                MemberCount = Convert.ToByte(body["memberCount"]),
                CompetitionId = Convert.ToInt64(body["competitionId"])
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `teams`(`title`, `organization`, `leader_id`, `escort_id`, `memberCount`, `competetion_id`) " +
                    $"VALUES ('{team.Title}', '{team.Organization}', '{team.LeaderId}', '{team.EscortId}', '{team.MemberCount}', '{team.CompetitionId}');";
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
                Title = body["title"],
                Number = Convert.ToByte(body["number"]),
                Organization = body["organization"],
                LeaderId = Convert.ToInt64(body["leaderId"]),
                EscortId = Convert.ToInt64(body["escortId"]),
                MemberCount = Convert.ToByte(body["memberCount"]),
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

            sql = $"UPDATE `teams` SET `title`='{team.Title}',`organization`='{team.Organization}',`leader_id`='{team.LeaderId}',`escort_id`='{team.EscortId}',`memberCount`='{team.MemberCount}',`points`='{team.Points}',`competetion_id`='{team.CompetitionId}' WHERE id={id};";
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
            sql = $"SELECT * FROM `teams` WHERE id={id}";
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
