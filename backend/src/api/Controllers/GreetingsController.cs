using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/greetings")]
    public class GreetingsController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("user")]
        public IActionResult Greet()
        {
            return Ok($"Hello {HttpContext.User.Identity.Name}");
        }

        [HttpGet]
        [Route("HelloWorld")]
        public IActionResult HelloWorld()
        {
            return Ok("Hello World!");
        }
    }
}
