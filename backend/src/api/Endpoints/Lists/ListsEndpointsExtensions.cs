namespace api.Endpoints.Lists;

public static class ListsEndpointsExtensions {
    public static IEndpointRouteBuilder MapListsEndpoints(this IEndpointRouteBuilder app){
        return app
        .MapGetAllListsEndpoint()
        .MapGetListEndpoint()
        .MapDeleteListEndpoint()
        .MapCreateListEndpoint()
        .MapRenameListEndpoint();
    }
}