using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    /// <summary>
    /// Controller for testing new features
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public TestController()
        {

        }

        [HttpGet("Ping")]
        public async Task<ActionResult> Ping()
        {
            return await Task.Run(() => Ok("PONG"));
        }
    }
}