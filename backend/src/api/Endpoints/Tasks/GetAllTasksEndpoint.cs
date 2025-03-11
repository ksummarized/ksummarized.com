using core;
using core.Ports;
using Serilog;

namespace api.Endpoints.Tasks;

public static class GetAllTasksEndpoint
{
    public const string Name = "GetAllTasks";
    public static IEndpointRouteBuilder MapGetAllTasksEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Tasks.GetAll, (
            ITodoService service,
            HttpContext context,
            int? tag,
            bool? completed) =>
        {
            var userId = (Guid)context.Items["UserId"]!;
            Log.Debug("User: {user} requested his items", userId);
            return Results.Ok(service.ListItems(userId, tag, completed));
        })
        .WithName(Name)
        .Produces<IEnumerable<TodoItem>>(StatusCodes.Status200OK)
        .RequireAuthorization();

        return app;
    }
}
