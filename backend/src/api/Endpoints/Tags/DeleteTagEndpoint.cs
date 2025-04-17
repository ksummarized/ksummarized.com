using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tags;

public static class DeleteTagEndpoint
{
    public const string Name = "DeleteTag";
    public static IEndpointRouteBuilder MapDeleteTagEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Todo.Tags.Delete, async (
            HttpContext ctx,
            int Id,
            [FromServices] ITagService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {User} deleted tag: {Id}", userId, Id);
            
            var success = await service.DeleteTag(userId, Id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
