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
    public IActionResult GetLists()
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} requested his lists", userId);
        return userId switch
        {
            null => Unauthorized(),
            var user => Ok(_service.GetLists(user)),
        };
    }

    [HttpPost("lists")]
    public async Task<IActionResult> CreateLists([FromQuery] string name)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} created: {list}", userId, name);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Create(user, name),
        };
        async Task<IActionResult> Create(string user, string name){
            var list = await _service.CreateList(user, name);
            return Created(HttpContext.Request.Path.Add(new PathString($"/{list.Id}")), list);
        }
    }
}