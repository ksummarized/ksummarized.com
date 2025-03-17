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
        (HttpContext ctx, int Id, [FromServices] ITodoService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {user} deleted his list: {id}", userId, Id);
            var success = service.DeleteList(userId, Id);
            if (success)
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
}