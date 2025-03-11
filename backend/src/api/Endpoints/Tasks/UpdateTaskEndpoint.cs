using core;
using core.Ports;
using Serilog;

namespace api.Endpoints.Tasks;

public static class UpdateTaskEndpoint
{
    public const string Name = "UpdateTask";
    public static IEndpointRouteBuilder MapUpdateTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Todo.Tasks.Update, async (
            ITodoService service,
            HttpContext context,
            int id,
            TodoItem request) =>
        {
            var userId = (Guid)context.Items["UserId"]!;
            Log.Debug("User: {user} updated his item: {id}", userId, id);
            
            var success = await service.UpdateItem(userId, request);
            return success ? Results.Ok() : Results.BadRequest();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();

        return app;
    }
}
