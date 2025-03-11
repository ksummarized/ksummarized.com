using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tasks;

public static class GetTaskEndpoint
{
    public const string Name = "GetTask";
    public static IEndpointRouteBuilder MapGetTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Tasks.Get, async (
            HttpContext ctx,
            int Id,
            [FromServices] ITodoService service) =>
        {
            var userId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} requested his item: {id}", userId, Id);
            
            var item = await service.GetItem(userId, Id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName(Name)
        .Produces<TodoItem>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
