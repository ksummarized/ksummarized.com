using core;
using core.Ports;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using api.Authorization;

namespace api.Endpoints.Tasks;

public static class GetAllTasksEndpoint
{
    public const string Name = "GetAllTasks";
    public static IEndpointRouteBuilder MapGetAllTasksEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Todo.Tasks.GetAll, (
            HttpContext ctx,
            [AsParameters] GetAllTasksRequest request,
            [FromServices] ITodoService service)=>
        {
            var userId = (Guid)ctx.Items["UserId"]!;
            Log.Debug("User: {user} requested his items", userId);
            return Results.Ok(service.ListItems(userId, request.Tag, request.Completed));
        })
        .WithName(Name)
        .Produces<IEnumerable<TodoItem>>(StatusCodes.Status200OK)
        .RequireAuthorization(UserIdRequirement.PolicyName);

        return app;
    }

    public class GetAllTasksRequest
    {
        public int? Tag { get; init; }
        public bool? Completed { get; init; }
    }
}
