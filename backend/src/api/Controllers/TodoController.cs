using core.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Resonses;
using core;

namespace api.Controllers;

[Authorize]
[Route("/api/todo")]
[ApiController]
public class TodoController(ITodoService service, ILogger<TodoController> logger) : ControllerBase
{
    private readonly ITodoService _service = service;
    private readonly ILogger<TodoController> _logger = logger;

    [HttpGet("lists")]
    public IActionResult GetLists()
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} requested his lists", userId);
        return userId switch
        {
            null => Unauthorized(),
            var user => Ok(_service.GetLists(user).Select(l => l.ToResponse())),
        };
    }

    [HttpGet("lists/{id}")]
    public IActionResult GetList([FromRoute] int id)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} requested his list: {id}", userId, id);
        if (userId is null) { return Unauthorized(); }
        var list = _service.GetList(userId, id)?.ToResponse();
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
    public async Task<IActionResult> CreateLists([FromBody] Request request)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} created: {list}", userId, request.Name);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Create(user, request.Name),
        };

        async Task<IActionResult> Create(string user, string name)
        {
            //TODO: return DTO instead of DAO
            var list = await _service.CreateList(user, name);
            return Created(HttpContext.Request.Path.Add(new PathString($"/{list.Id}")), list.ToResponse());
        }
    }

    [HttpPut("lists/{id}")]
    public async Task<IActionResult> RenameList(RenameRequest request)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} renamed: {id} to: {list}", userId, request.Id, request.Body.Name);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Rename(user, request.Id, request.Body.Name),
        };

        async Task<IActionResult> Rename(string user, int id, string name)
        {
            //TODO: return DTO instead of DAO
            var list = await _service.RenameList(user, id, name);
            if (list)
            {
                return Ok();
            }
            return BadRequest();
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody] TodoItem request)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} created: {item}", userId, request.Name);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Handle(user, request),
        };

        async Task<IActionResult> Handle(string user, TodoItem item)
        {
            var newItem = await _service.CreateItem(user, item);
            if (newItem is null)
            {
                return BadRequest();
            }
            return Created(HttpContext.Request.Path.Add(new PathString($"/{newItem.Id}")), newItem);
        }
    }

    [HttpGet("items")]
    public IActionResult ListItems()
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} requested his items", userId);
        return userId switch
        {
            null => Unauthorized(),
            var user => Ok(_service.ListItems(user)),
        };
    }

    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} requested his item: {id}", userId, id);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Handle(user, id),
        };

        async Task<IActionResult> Handle(string user, int id)
        {
            var item = await _service.GetItem(user, id);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(item);
        }
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem([FromRoute] int id)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} deleted his item: {id}", userId, id);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Handle(user, id),
        };

        async Task<IActionResult> Handle(string user, int id)
        {
            var success = await _service.DeleteItem(user, id);
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem([FromRoute] int id, [FromBody] TodoItem request)
    {
        var userId = Request.UserId();
        _logger.LogDebug("User: {user} updated his item: {id}", userId, id);
        return userId switch
        {
            null => Unauthorized(),
            var user => await Handle(user, id),
        };

        async Task<IActionResult> Handle(string user, int id)
        {
            var success = await _service.UpdateItem(user, request);
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}

public class Request
{
    public required string Name { get; set; }
}

public class RenameRequest
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
