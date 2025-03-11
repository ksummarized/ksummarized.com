namespace api.Endpoints.Tasks;

public static class TasksEndpointsExtensions 
{
    public static IEndpointRouteBuilder MapTasksEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateTaskEndpoint()
            .MapGetAllTasksEndpoint()
            .MapGetTaskEndpoint()
            .MapDeleteTaskEndpoint()
            .MapUpdateTaskEndpoint();
    }
}