using api.Authorization;
using core.Ports;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.Endpoints.Lists;
public static class RenameListEndpoint
{
    public const string Name = "RenameList";
    public static IEndpointRouteBuilder MapRenameListEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Todo.Lists.Rename, async (HttpContext ctx, [FromRoute] int Id, [FromBody] ListRenameRequest request, [FromServices] ITodoService service) =>
        {
            var UserId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} renamed: {id} to: {list}", UserId, Id, request.Name);
            var list = await service.RenameList(UserId, Id, request.Name);
            if (list)
            {
                return Results.Ok();
            }
            return Results.NotFound();


        })
        .WithName(Name)
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);
        return app;
    }

    public class ListRenameRequest
    {
        public required string Name { get; set; }
    }
}