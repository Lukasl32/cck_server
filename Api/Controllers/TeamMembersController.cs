using System.Data.Common;

using Accessories.Enums;
using Accessories.Models;

using Microsoft.AspNetCore.Mvc;

using MySqlConnector;
using Microsoft.Extensions.Primitives;
using Accessories.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/teams/members")]
    [ApiController]
    public class TeamMembersController : ControllerBase
    {
        [HttpGet]
        public async Task<List<UserTeamMember>> Get()
        {
            Security.Authorize(HttpContext);

            var teamMembers = new List<UserTeamMember>();

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `team_id`, `firstName`, `lastName`, `type`+0, `phone_number`, `birthdate` FROM `team_members`;";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                teamMembers.Add(new UserTeamMember
                {
                    Id = reader.GetInt64(0),
                    TeamId = reader.GetInt64(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Type = (TeamMemberType)reader.GetInt32(4),
                    PhoneNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Birthdate = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                });
            }
            return teamMembers;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Security.Authorize(HttpContext);

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql = $"SELECT `id`, `team_id`, `firstName`, `lastName`, `type`+0, `phone_number`, `birthdate` FROM `team_members` WHERE id={id};";
            using MySqlCommand command = new(sql, connection);
            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new UserTeamMember
                {
                    Id = reader.GetInt64(0),
                    TeamId = reader.GetInt64(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Type = (TeamMemberType)reader.GetInt32(4),
                    PhoneNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Birthdate = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
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

            UserTeamMember teamMember = new()
            {
                TeamId = Convert.ToInt64(body["teamId"]),
                FirstName = body["firstName"],
                LastName = body["lastName"],
                Type = (TeamMemberType)Convert.ToInt32(body["type"]),
                PhoneNumber = body["phoneNumber"] == "" ? StringValues.Empty : body["phoneNumber"],
                Birthdate = (body["birthdate"] == StringValues.Empty || body["birthdate"] == "NULL" || body["birthdate"] == "") ? null : Convert.ToDateTime(body["birthdate"]),
            };

            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO `team_members`(`team_id`, `firstName`, `lastName`, `type`,  `signature`, `phone_number`, `birthdate`) " +
                    $"VALUES ('{teamMember.TeamId}', '{teamMember.FirstName}', '{teamMember.LastName}', '{(int)teamMember.Type}', {Sql.Nullable(teamMember.Type == TeamMemberType.Velitel ? Security.GenerateHash(512) : null)}, {Sql.Nullable(teamMember.PhoneNumber)}, {Sql.Nullable(teamMember.Birthdate)});";
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

        // PUT api/<TeamMembersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            Security.Authorize(HttpContext);

            var body = HttpContext.Request.Form;

            UserTeamMember teamMember = new()
            {
                TeamId = Convert.ToInt64(body["teamId"]),
                FirstName = body["firstName"],
                LastName = body["lastName"],
                Type = (TeamMemberType)Convert.ToInt32(body["type"]),
                PhoneNumber = body["phoneNumber"] == "" ? StringValues.Empty : body["phoneNumber"],
                Birthdate = (body["birthdate"] == StringValues.Empty || body["birthdate"] == "NULL" || body["birthdate"] == "") ? null : Convert.ToDateTime(body["birthdate"]),
            };

            using MySqlConnection connection = new(Config.ConnString);
            await connection.OpenAsync();
            string sql;

            //kontrola že uživatel s ID je PRÁVĚ jeden
            sql = $"SELECT COUNT(*) FROM `team_members` WHERE id={id}";
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

            sql = $"UPDATE `team_members` SET `team_id`='{teamMember.TeamId}',`firstName`='{teamMember.FirstName}',`lastName`='{teamMember.LastName}',`type`='{(int)teamMember.Type}',`phone_number`={Sql.Nullable(teamMember.PhoneNumber)},`birthdate`={Sql.Nullable(teamMember.Birthdate)} WHERE id={id};";
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
            sql = $"SELECT COUNT(*) FROM `team_members` WHERE id={id}";
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

            sql = $"DELETE FROM `team_members` WHERE id={id};";
            using (MySqlCommand command = new(sql, connection))
            {
                await command.ExecuteReaderAsync();
                return Ok();
            }
        }
    }
}
