namespace api.Endpoints.Tags;

public static class TagsEndpointsExtensions 
{
    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateTagEndpoint()
            .MapGetAllTagsEndpoint()
            .MapGetTagEndpoint()
            .MapUpdateTagEndpoint()
            .MapDeleteTagEndpoint();
    }
}
