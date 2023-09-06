using api.Data.DTO;
using api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
    {
        _userService = userService;
        _logger = logger;

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

        _logger.LogInformation("User {user} tried to log in", user.Email);
        var (IsSuccess, AuthResult, Error) = await _userService.Login(user);
        if (IsSuccess)
        {
            _logger.LogInformation("User {user} loged in.", user.Email);
            return Ok(AuthResult);
        }
        else
        {
            _logger.LogInformation("Failed login for {User}", user.Email);
            return Unauthorized(Error);
        }
    }

    [HttpGet("login-github-callback")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> LoginWithGithubCallback([FromQuery] string code)
    {
        _logger.LogInformation("Got the code: {code} in a callback", code);
        using var client = new HttpClient();

        var body = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("client_id", "b92b25beec92bb9902f7"),
                new KeyValuePair<string, string>("client_secret", "6a2987c98794d90d0a3d27d22b8c048843ee5229"),
                new KeyValuePair<string, string>("code", code),
        });
        using var tokenRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://github.com/login/oauth/access_token"),
            Content = body,
        };
        tokenRequest.Headers.Add("Accept", "application/json");
        var token = JsonSerializer.Deserialize<GitHubTokenResponse>(await (await client.SendAsync(tokenRequest)).Content.ReadAsStringAsync());
        _logger.LogInformation("Got the following token {token}", JsonSerializer.Serialize(token));

        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://api.github.com/user/emails")
        };
        request.Headers.Add("Authorization", $"Bearer {token.access_token}");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "KSummarized");
        _logger.LogInformation("----");
        var tmp = await (await client.SendAsync(request)).Content.ReadAsStringAsync();
        _logger.LogInformation("TMP {body}", tmp);
        var user = JsonSerializer.Deserialize<List<GitHubEmailsResponse>>(tmp);
        _logger.LogInformation("Got following data: {user}", JsonSerializer.Serialize(user));

        var email = user.Single(u => u.primary == true).email;
        _logger.LogInformation("Email: {email}");
        var (IsSuccess, AuthResult, Error) = await _userService.OAuthLogin(email);
        if (IsSuccess)
        {
            _logger.LogInformation("User {user} loged in.", email);
            return Ok(AuthResult);
        }
        else
        {
            _logger.LogInformation("Failed login for {User}", email);
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
