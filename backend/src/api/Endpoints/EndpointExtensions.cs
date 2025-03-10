using api.Endpoints.Lists;
using api.Endpoints.Tasks;

namespace api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapListsEndpoints();
        app.MapTasksEndpoints();
        return app;
    }
}