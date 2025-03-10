using core.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using core;
using api.Mapers;
using api.Authorization;

namespace api.Controllers;

[Authorize]
[Route("/api/todo")]
[ApiController]
public partial class TodoController(ITodoService service, ILogger<TodoController> logger) : ControllerBase
{
    private readonly ITodoService _service = service;
    private readonly ILogger<TodoController> _logger = logger;

    //We are sure that this is not null because of the [UserIdFilter]
    private Guid UserId => (Guid)HttpContext.Items["UserId"]!;

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody] TodoItem request)
    {
        _logger.LogDebug("User: {user} created: {item}", UserId, request.Name);
        var newItem = await _service.CreateItem(UserId, request);
        if (newItem is null)
        {
            return BadRequest();
        }
        return Created(HttpContext.Request.Path.Add(new PathString($"/{newItem.Id}")), newItem);
    }

    [HttpGet("items")]
    public IActionResult ListItems([FromQuery]ListItemsRequest? request)
    {
        _logger.LogDebug("User: {user} requested his items", UserId);
        return Ok(_service.ListItems(UserId, request?.Tag, request?.Compleated));
    }

    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
        _logger.LogDebug("User: {user} requested his item: {id}", UserId, id);
        var item = await _service.GetItem(UserId, id);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem([FromRoute] int id)
    {
        _logger.LogDebug("User: {user} deleted his item: {id}", UserId, id);
        var success = await _service.DeleteItem(UserId, id);
        if (success)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem([FromRoute] int id, [FromBody] TodoItem request)
    {
        _logger.LogDebug("User: {user} updated his item: {id}", UserId, id);
        var success = await _service.UpdateItem(UserId, request);
        if (success)
        {
            return Ok();
        }
        return BadRequest();
    }
}
