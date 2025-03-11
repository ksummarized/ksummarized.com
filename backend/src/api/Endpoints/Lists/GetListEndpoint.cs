using api.Authorization;
using api.Mapers;
using core;
using core.Ports;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.Endpoints.Lists;

public static class GetListEndpoint
{
    public const string Name = "GetList";
    public static IEndpointRouteBuilder MapGetListEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Lists.Get, (
            HttpContext ctx,
            [FromRoute] int Id,
            [AsParameters] GetListRequest request,
            [FromServices] ITodoService service) =>
        {
            var UserId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} requested his list: {id}", UserId, Id);
            var list = service.GetList(request.ToGetListOptions(Id, UserId))?.ToResponse();
            return list switch
            {
                null => Results.NotFound(),
                var l => Results.Ok(l),
            };
        })
        .WithName(Name)
        .Produces<GetListResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(UserIdRequirement.PolicyName);
        return app;
    }

    public class GetListRequest : PaginatedRequest
    {
        public const bool DefaultIncludeSubtasks = true;

        public int? Tag { get; init; }
        public bool? Compleated { get; init; }
        public bool? IncludeSubtasks { get; init; } = DefaultIncludeSubtasks;
    }

    public record GetListResponse(int Id, string Name, IEnumerable<TodoItem> Items);
}