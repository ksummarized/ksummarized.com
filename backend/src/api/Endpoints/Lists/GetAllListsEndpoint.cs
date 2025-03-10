using api.Authorization;
using api.Mapers;
using core.Ports;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static api.Endpoints.Lists.GetListEndpoint;

namespace api.Endpoints.Lists;

public static class GetAllListsEndpoint
{
    public const string Name = "GetAllLists";
    public static IEndpointRouteBuilder MapGetAllListsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Lists.GetAll, (HttpContext ctx, [FromServices] ITodoService service) =>
        {
            var UserId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} requested his lists", UserId);
            return Results.Ok(service.GetLists(UserId).Select(l => l.ToResponse()));
        })
        .WithName(Name)
        .Produces<IEnumerable<GetListResponse>>(StatusCodes.Status200OK)
        .RequireAuthorization(UserIdRequirement.PolicyName);
        return app;
    }
}