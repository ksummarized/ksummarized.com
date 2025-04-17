using api.Authorization;
using core.Ports;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.Endpoints.Lists;

public static class DeleteListEndpoint
{
    public const string Name = "DeleteList";
    public static IEndpointRouteBuilder MapDeleteListEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Todo.Lists.Delete,
        (HttpContext ctx, int Id, [FromServices] IListService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {User} deleted his list: {Id}", userId, Id);
            var success = service.DeleteList(userId, Id);
            if (success)
            {
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName(Name)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);
        return app;
    }
}