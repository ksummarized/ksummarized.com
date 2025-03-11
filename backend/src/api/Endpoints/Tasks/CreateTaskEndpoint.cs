using core;
using core.Ports;
using Serilog;

namespace api.Endpoints.Tasks;

public static class CreateTaskEndpoint
{
    public const string Name = "CreateTask";
    public static IEndpointRouteBuilder MapCreateTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Todo.Tasks.Create, async (
            ITodoService service,
            TodoItem request,
            HttpContext context) =>
        {
            var userId = (Guid)context.Items["UserId"]!;
            Log.Debug("User: {user} created: {item}", userId, request.Name);
            
            var newItem = await service.CreateItem(userId, request);
            if (newItem is null)
            {
                return Results.BadRequest();
            }
            return Results.Created($"{context.Request.Path}/{newItem.Id}", newItem);
        })
        .WithName(Name)
        .Produces<TodoItem>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();

        return app;
    }
}
