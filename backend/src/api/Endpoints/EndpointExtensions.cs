using api.Endpoints.Lists;
using api.Endpoints.Tasks;
using api.Endpoints.Tags;

namespace api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapListsEndpoints();
        app.MapTasksEndpoints();
        app.MapTagsEndpoints();
        return app;
    }
}