using api.Constants;
using api.Data.DTO;
using api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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

    [HttpGet("create-user")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser()
    {
        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var jsonTokenData = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var keycloakUuid = jsonTokenData.Subject;
        var userEmail = jsonTokenData.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

        if (userEmail == null)
        {
            _logger.LogInformation("User with keycloakUuid={keycloakUuid} tried to log in without email.", keycloakUuid);
            return BadRequest("Required data missing");
        }

        var (isSuccess, error) = await _userService.CreateKeycloakUser(
            new UserDto
            {
                KeycloakUuid = keycloakUuid,
                Email = userEmail
            }
        );

        if (isSuccess)
        {
            _logger.LogInformation("The user {email} has been created successfully.", userEmail);
            return Ok("User created");
        }
        else
        {
            if (error == ErrorMessages.UserAlreadyExists)
            {
                _logger.LogInformation("The user {email} has already been created.", userEmail);
                return Ok("User is already created");
            }
            _logger.LogInformation("Failure during creation of {email} user.", userEmail);
            return BadRequest(error);
        }
    }
}
