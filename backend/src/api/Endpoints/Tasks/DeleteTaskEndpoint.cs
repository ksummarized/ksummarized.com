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
            [FromRoute] int Id,
            HttpContext ctx,
            [FromServices] ITodoService service) =>
        {
            var userId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} deleted his item: {id}", userId, Id);
            
            var success = await service.DeleteItem(userId, Id);
            return success ? Results.Ok() : Results.NotFound();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
