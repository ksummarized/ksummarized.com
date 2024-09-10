using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
[Route("/api/todo")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService service, ILogger<TodoController> logger){
        _service = service;
        _logger = logger;
    }

    [HttpGet("lists")]
    public IActionResult List()
    {
        var userId = Request.UserId();
        _logger.LogInformation("User: {user} requested his lists", userId);
        return userId switch
        {
            null => Unauthorized(),
            var user => Ok(_service.GetLists(user)),
        };
    }

}