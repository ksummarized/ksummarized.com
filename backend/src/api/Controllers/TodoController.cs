using core.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Responses;
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
    [ProducesResponseType(typeof(IEnumerable<Responses.TodoList>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public IActionResult GetLists()
    {
        _logger.LogDebug("User: {UserId} requested his lists", UserId);
        return Ok(_service.GetLists(UserId).Select(l => l.ToResponse()));
    }

    [HttpGet("lists/{id}")]
    [ProducesResponseType(typeof(Responses.TodoList), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public IActionResult GetList([FromRoute] int id)
    {
        _logger.LogDebug("User: {UserId} requested his list: {ListId}", UserId, id);
        var list = _service.GetList(UserId, id)?.ToResponse();
        return list switch
        {
            null => NotFound(),
            var l => Ok(l),
        };
    }

    [HttpDelete("lists/{id}")]
    public IActionResult DeleteList([FromRoute] int id)
    {
        _logger.LogDebug("User: {UserId} deleted his list: {ListId}", UserId, id);
        var success = _service.DeleteList(UserId, id);
        if (success)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpPost("lists")]
    [ProducesResponseType(typeof(Responses.TodoList), StatusCodes.Status201Created)]
    [Produces("application/json")]
    public async Task<IActionResult> CreateLists([FromBody] ListCreationRequest request)
    {
        _logger.LogDebug("User: {UserId} created: {ListName}", UserId, request.Name);
        var list = await _service.CreateList(UserId, request.Name);
        return Created(HttpContext.Request.Path.Add(new PathString($"/{list.Id}")), list.ToResponse());
    }

    [HttpPut("lists/{id}")]
    public async Task<IActionResult> RenameList(ListRenameRequest request)
    {
        _logger.LogDebug("User: {UserId} renamed: {ListId} to: {NewListName}", UserId, request.Id, request.Body.Name);
        var list = await _service.RenameList(UserId, request.Id, request.Body.Name);
        if (list)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status201Created)]
    [Produces("application/json")]
    public async Task<IActionResult> CreateItem([FromBody] TodoItem request)
    {
        _logger.LogDebug("User: {UserId} created: {ItemName}", UserId, request.Name);
        var newItem = await _service.CreateItem(UserId, request);
        if (newItem is null)
        {
            return BadRequest();
        }
        return Created(HttpContext.Request.Path.Add(new PathString($"/{newItem.Id}")), newItem);
    }

    [HttpGet("items")]
    [ProducesResponseType(typeof(IEnumerable<TodoItem>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public IActionResult ListItems()
    {
        _logger.LogDebug("User: {UserId} requested his items", UserId);
        return Ok(_service.ListItems(UserId));
    }

    [HttpGet("items/{id}")]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
        _logger.LogDebug("User: {UserId} requested his item: {ItemId}", UserId, id);
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
        _logger.LogDebug("User: {UserId} deleted his item: {ItemId}", UserId, id);
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
        _logger.LogDebug("User: {UserId} updated his item: {ItemId}", UserId, id);
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
