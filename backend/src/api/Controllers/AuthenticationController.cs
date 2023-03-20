using api.Data.DTO;
using api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] UserDTO user)
    {
        if (!ModelState.IsValid) { return BadRequest("Invalid data provided!"); }

        var (IsSuccess, Error) = await _userService.Register(user);
        if (IsSuccess)
        {
            return Ok("User created");
        }
        else
        {
            return BadRequest(Error);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] UserDTO user)
    {
        if (!ModelState.IsValid) { return BadRequest("Please provide login credentials!"); }

        var (IsSuccess, AuthResult, Error) = await _userService.Login(user);
        if (IsSuccess)
        {
            return Ok(AuthResult);
        }
        else
        {
            return Unauthorized(Error);
        }
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequestDTO)
    {
        if (!ModelState.IsValid) { return BadRequest("Invalid token request."); }
        var (IsSuccess, AuthResult, Error) = await _userService.RefreshLogin(tokenRequestDTO);
        if (IsSuccess)
        {
            return Ok(AuthResult);
        }
        else
        {
            return Unauthorized(Error);
        }
    }

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    [Authorize()]
    public async Task<IActionResult> Logout()
    {
        if (HttpContext.User?.Identity?.Name is not null)
        {
            await _userService.Logout(HttpContext.User.Identity.Name);
        }
        return Ok("The user has been logged out.");
    }

}
