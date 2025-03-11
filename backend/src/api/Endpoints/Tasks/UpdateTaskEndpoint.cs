using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tasks;

public static class UpdateTaskEndpoint
{
    public const string Name = "UpdateTask";
    public static IEndpointRouteBuilder MapUpdateTaskEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Todo.Tasks.Update, async (
            [FromServices] ITodoService service,
            [FromBody] TodoItem request,
            HttpContext ctx) =>
        {
            var userId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} updated his item: {id}", userId, request.Id);
            
            var success = await service.UpdateItem(userId, request);
            return success ? Results.Ok() : Results.NotFound();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
