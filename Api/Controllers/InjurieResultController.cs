using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/result/injurie")]
    [ApiController]
    public class InjurieResultController : ControllerBase
    {
        [HttpGet]
        public async Task<List<object>> Get()
        {

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {

        }
    }
}
