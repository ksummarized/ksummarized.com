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

    public TodoController(ITodoService service, ILogger<TodoController> logger)
    {
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

    [HttpGet("lists/{id}")]
    public IActionResult GetList([FromRoute] int id)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} requested his list: {id}", userId, id);
        if (userId is null) { return Unauthorized(); }
        var list = _service.GetList(userId, id);
        return list switch
        {
            null => NotFound(),
            var user => Ok(list),
        };
    }

    [HttpDelete("lists/{id}")]
    public IActionResult DeleteList([FromRoute] int id)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} deleted his list: {id}", userId, id);
        if (userId is null) { return Unauthorized(); }
        var success = _service.DeleteList(userId, id);
        if (success)
        {
            return Ok();
        }
        return BadRequest();
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

        async Task<IActionResult> Create(string user, string name)
        {
            //TODO: return DTO instead of DAO
            var list = await _service.CreateList(user, name);
            return Created(HttpContext.Request.Path.Add(new PathString($"/{list.Id}")), list);
        }
    }
}
