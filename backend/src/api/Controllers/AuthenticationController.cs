using api.Data;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService) => _userService = userService;

        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody]UserVM user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided!");
            }
            var result = await _userService.Register(user);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("User already exists!");
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody]UserVM user)
        {
            if (!ModelState.IsValid) { return BadRequest("Please provide login credentials!"); }
            var result = await _userService.Login(user);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized("Invalid email or password");
            }
        }
    }
}
