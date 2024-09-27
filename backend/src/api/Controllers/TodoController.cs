using core.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Resonses;
using api.Filters;
using core;

namespace api.Controllers;

[Authorize]
[UserIdFilter]
[Route("/api/todo")]
[ApiController]
public class TodoController(ITodoService service, ILogger<TodoController> logger) : ControllerBase
{
    private readonly ITodoService _service = service;
    private readonly ILogger<TodoController> _logger = logger;

    //We are sure that this is not null because of the [UserIdFilter]
    private Guid UserId => (Guid)HttpContext.Items["UserId"]!;

    [HttpGet("lists")]
    public IActionResult GetLists()
    {
        _logger.LogDebug("User: {user} requested his lists", UserId);
        return Ok(_service.GetLists(UserId).Select(l => l.ToResponse()));
    }

    [HttpGet("lists/{id}")]
    public IActionResult GetList([FromRoute] int id)
    {
        _logger.LogDebug("User: {user} requested his list: {id}", UserId, id);
        var list = _service.GetList(UserId, id)?.ToResponse();
        return list switch
        {
            null => NotFound(),
            var user => Ok(list),
        };
    }

    [HttpDelete("lists/{id}")]
    public IActionResult DeleteList([FromRoute] int id)
    {
        _logger.LogDebug("User: {user} deleted his list: {id}", UserId, id);
        var success = _service.DeleteList(UserId, id);
        if (success)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpPost("lists")]
    public async Task<IActionResult> CreateLists([FromBody] ListCreationRequest request)
    {
        _logger.LogDebug("User: {user} created: {list}", UserId, request.Name);
        var list = await _service.CreateList(UserId, request.Name);
        return Created(HttpContext.Request.Path.Add(new PathString($"/{list.Id}")), list.ToResponse());
    }

    [HttpPut("lists/{id}")]
    public async Task<IActionResult> RenameList(ListRenameRequest request)
    {
        _logger.LogDebug("User: {user} renamed: {id} to: {list}", UserId, request.Id, request.Body.Name);
        var list = await _service.RenameList(UserId, request.Id, request.Body.Name);
        if (list)
        {
            return Ok();
        }
        return BadRequest();
    }

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
    public IActionResult ListItems()
    {
        _logger.LogDebug("User: {user} requested his items", UserId);
        return Ok(_service.ListItems(UserId));
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

    public class ListCreationRequest
    {
        public required string Name { get; set; }
    }

    public class ListRenameRequest
    {
        [FromRoute]
        public required int Id { get; set; }
        [FromBody]
        public required Payload Body { get; set; }

        public class Payload
        {
            public required string Name { get; set; }
        }
    }
}
