namespace api.Endpoints.Lists;

public static class ListsEndpointsExtensions {
    public static IEndpointRouteBuilder MapListsEndpoints(this IEndpointRouteBuilder app){
        app.MapGetListEndpoint();
        return app;
    }
}