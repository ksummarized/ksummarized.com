using core;
using core.Ports;
using Serilog;
using api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints.Tags;

public static class GetAllTagsEndpoint
{
    public const string Name = "GetAllTags";
    public static IEndpointRouteBuilder MapGetAllTagsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Tags.GetAll, (
            HttpContext ctx,
            [FromServices] ITodoService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {user} requested all tags", userId);
            return Results.Ok(service.ListTags(userId));
        })
        .WithName(Name)
        .Produces<IEnumerable<Tag>>(StatusCodes.Status200OK)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }
}
