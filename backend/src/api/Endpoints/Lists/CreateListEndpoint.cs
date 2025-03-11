using api.Authorization;
using api.Mapers;
using core.Ports;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static api.Endpoints.Lists.GetListEndpoint;

namespace api.Endpoints.Lists;

public static class CreateListEndpoint
{
    public const string Name = "CreateList";
    public static IEndpointRouteBuilder MapCreateListEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Todo.Lists.Create,
        async (HttpContext ctx, CreateListRequest request, [FromServices] ITodoService service) =>
        {
            var UserId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} created: {list}", UserId, request.Name);
            var list = await service.CreateList(UserId, request.Name);
            return TypedResults.CreatedAtRoute(list.ToResponse(), GetListEndpoint.Name, new { Id = list.Id });
        })
        .Produces<GetListResponse>(StatusCodes.Status201Created)
        .RequireAuthorization(UserIdRequirement.PolicyName);
        return app;
    }

    public record CreateListRequest(string Name);
}