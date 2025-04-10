using contracts.Requests;
using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tags;

public static class CreateTagEndpoint
{
    public const string Name = "CreateTag";
    public static IEndpointRouteBuilder MapCreateTagEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Todo.Tags.Create, async (
            HttpContext ctx,
            CreateTagRequest request,
            [FromServices] ITagService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {user} created tag: {name}", userId, request.Name);
            
            var tag = await service.CreateTag(userId, request.Name);
            return TypedResults.CreatedAtRoute(tag, GetTagEndpoint.Name, new { Id = tag.Id });
        })
        .WithName(Name)
        .Produces<Tag>(StatusCodes.Status201Created)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }

}
