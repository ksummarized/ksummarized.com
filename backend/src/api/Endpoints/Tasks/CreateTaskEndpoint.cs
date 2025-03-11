using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tasks;

public static class CreateTaskEndpoint
{
    public const string Name = "CreateTask";
    public static IEndpointRouteBuilder MapCreateTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Todo.Tasks.Create, async (
            HttpContext ctx,
            [FromServices] ITodoService service,
            [FromBody] TodoItem request) =>
        {
            var userId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} created: {item}", userId, request.Name);
            
            var newItem = await service.CreateItem(userId, request);
            if (newItem is null)
            {
                return Results.BadRequest();
            }
            return TypedResults.CreatedAtRoute(newItem, GetTaskEndpoint.Name, new { Id = newItem.Id! });
        })
        .WithName(Name)
        .Produces<TodoItem>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
