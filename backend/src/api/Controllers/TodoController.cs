using api.Data.DTO;
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

    public TodoController(ITodoService service) => _service = service;

    [HttpGet("lists")]
    public IActionResult List()
    {
        return Request.UserId() switch
        {
            null => Unauthorized(),
            var user => Ok(_service.GetLists(user)),
        };
    }

}
