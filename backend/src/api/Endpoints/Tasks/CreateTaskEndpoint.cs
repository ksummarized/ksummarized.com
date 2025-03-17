using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Mapers;
using contracts.Requests;

namespace api.Endpoints.Tasks;

public static class CreateTaskEndpoint
{
    public const string Name = "CreateTask";
    public static IEndpointRouteBuilder MapCreateTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Todo.Tasks.Create, async (
            HttpContext ctx,
            CreateTaskRequest request,
            [FromServices] ITodoService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {user} created: {item}", userId, request.Name);

            var newItem = await service.CreateItem(userId, request.MapToTodoItem());
            if (newItem is null)
            {
                return Results.BadRequest();
            }
            return TypedResults.CreatedAtRoute(newItem.MapToResponse(), GetTaskEndpoint.Name, new { Id = newItem.Id! });
        })
        .WithName(Name)
        .Produces<TodoItem>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
