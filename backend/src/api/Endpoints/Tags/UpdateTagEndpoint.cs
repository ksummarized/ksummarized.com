using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tags;

public static class UpdateTagEndpoint
{
    public const string Name = "UpdateTag";
    public static IEndpointRouteBuilder MapUpdateTagEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Todo.Tags.Update, async (
            HttpContext ctx,
            int Id,
            UpdateTagRequest request,
            [FromServices] ITodoService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {user} updated tag: {id}", userId, Id);
            
            var success = await service.UpdateTag(userId, Id, request.Name);
            return success ? Results.Ok() : Results.NotFound();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }

    public record UpdateTagRequest(string Name);
}
