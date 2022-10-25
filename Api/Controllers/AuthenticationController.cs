using Accessories.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using MySqlConnector;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public async Task<IActionResult> Login([FromBody] string email, [FromBody] string password)
        {
            string? passwordHash;
            using (MySqlConnection connection = new(Config.ConnString))
            {
                await connection.OpenAsync();
                string sql = $"SELECT `password` FROM `user` WHERE email='{email}';";
                using MySqlCommand command = new(sql, connection);
                passwordHash = Convert.ToString(await command.ExecuteScalarAsync());
            }

            if(BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, BCrypt.Net.HashType.SHA512))
            {
                Token token;
                return Ok(token);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
