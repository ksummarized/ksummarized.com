using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tasks;

public static class DeleteTaskEndpoint
{
    public const string Name = "DeleteTask";
    public static IEndpointRouteBuilder MapDeleteTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Todo.Tasks.Delete, async (
            HttpContext ctx,
            int Id,
            [FromServices] IItemService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {User} deleted his item: {Id}", userId, Id);
            
            var success = await service.DeleteItem(userId, Id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
