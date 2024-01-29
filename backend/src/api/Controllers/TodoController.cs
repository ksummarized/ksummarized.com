using api.Data.DTO.ToDo;
using api.Services.ToDo;
using api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api;

[Authorize]
[ApiController]
[Route("/api/todo")]
public class TodoController : ControllerBase
{
    private readonly IToDoListService _toDoListService;
    private readonly IUserService _userService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(IToDoListService toDoListService, ILogger<TodoController> logger, IUserService userService)
    {
        _toDoListService = toDoListService;
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("lists")]
    public async Task<IActionResult> GetLists()
    {
        if (User.Identity?.IsAuthenticated ?? false && User.Identity.Name is not null)
        {
            var id = await _userService.GetUserId(User.Identity.Name!);
            _logger.LogInformation("User: {UserId} is reading lists", id);
            var lists = await _toDoListService.GetToDoLists(id!);
            return Ok(lists);
        }
        return Unauthorized();
    }

    [HttpPost("lists")]
    public async Task<IActionResult> CreateLists([FromBody] string name)
    {
        if (User.Identity?.IsAuthenticated ?? false && User.Identity.Name is not null)
        {
            var id = await _userService.GetUserId(User.Identity.Name!);
            _logger.LogInformation("User: {UserId} is creating a list: {List}", id, name);
            var list = await _toDoListService.CreateList(id!, name);
            return Created(new Uri($"/api/todo/lists/{list.Id}"), list);
        }
        return Unauthorized();
    }

    [HttpPost("lists/{id}")]
    public async Task<IActionResult> EditLists([FromRoute] Guid id, [FromBody] string newName)
    {
        if (User.Identity?.IsAuthenticated ?? false && User.Identity.Name is not null)
        {
            var userId = await _userService.GetUserId(User.Identity.Name!);
            _logger.LogInformation("User: {UserId} is editing a list: {List}", userId, newName);
            await _toDoListService.EditList(id, newName);
            return Ok();
        }
        return Unauthorized();
    }

    [HttpDelete("lists/{id}")]
    public async Task<IActionResult> DeleteLists([FromRoute] Guid id)
    {
        if (User.Identity?.IsAuthenticated ?? false && User.Identity.Name is not null)
        {
            var userId = await _userService.GetUserId(User.Identity.Name!);
            _logger.LogInformation("User: {UserId} is deleting a list: {id}", userId, id);
            await _toDoListService.DeleteList(id);
            return Ok();
        }
        return Unauthorized();
    }
}
