using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tags;

public static class GetTagEndpoint
{
    public const string Name = "GetTag";
    public static IEndpointRouteBuilder MapGetTagEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Tags.Get, async (
            HttpContext ctx,
            int Id,
            [FromServices] ITodoService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {user} requested tag: {id}", userId, Id);
            
            var tag = await service.GetTag(userId, Id);
            return tag is null ? Results.NotFound() : Results.Ok(tag);
        })
        .WithName(Name)
        .Produces<Tag>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
