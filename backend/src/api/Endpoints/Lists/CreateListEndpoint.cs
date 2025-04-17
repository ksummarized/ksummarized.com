using api.Authorization;
using api.Mapers;
using core.Ports;
using contracts.Requests;
using contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.Endpoints.Lists;

public static class CreateListEndpoint
{
    public const string Name = "CreateList";
    public static IEndpointRouteBuilder MapCreateListEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Todo.Lists.Create,
        async (HttpContext ctx, CreateListRequest request, [FromServices] IListService service) =>
        {
            var userId = ctx.UserId();
            Log.Debug("User: {User} created: {List}", userId, request.Name);
            var list = await service.CreateList(userId, request.Name);
            return TypedResults.CreatedAtRoute(list.ToResponse(), GetListEndpoint.Name, new { Id = list.Id });
        })
        .Produces<GetListResponse>(StatusCodes.Status201Created)
        .RequireAuthorization(UserIdRequirement.PolicyName);
        return app;
    }
}