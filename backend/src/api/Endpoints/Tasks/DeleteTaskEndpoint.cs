using core.Ports;
using Serilog;

namespace api.Endpoints.Tasks;

public static class DeleteTaskEndpoint
{
    public const string Name = "DeleteTask";
    public static IEndpointRouteBuilder MapDeleteTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Todo.Tasks.Delete, async (
            ITodoService service,
            HttpContext context,
            int id) =>
        {
            var userId = (Guid)context.Items["UserId"]!;
            Log.Debug("User: {user} deleted his item: {id}", userId, id);
            
            var success = await service.DeleteItem(userId, id);
            return success ? Results.Ok() : Results.BadRequest();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();

        return app;
    }
}
