using core;
using core.Ports;
using Serilog;

namespace api.Endpoints.Tasks;

public static class GetTaskEndpoint
{
    public const string Name = "GetTask";
    public static IEndpointRouteBuilder MapGetTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Tasks.Get, async (
            ITodoService service,
            HttpContext context,
            int id) =>
        {
            var userId = (Guid)context.Items["UserId"]!;
            Log.Debug("User: {user} requested his item: {id}", userId, id);
            
            var item = await service.GetItem(userId, id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName(Name)
        .Produces<TodoItem>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        return app;
    }
}
