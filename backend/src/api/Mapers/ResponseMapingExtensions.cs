using static api.Endpoints.Lists.GetListEndpoint;

namespace api.Mapers;

public static class ResponseMapingExtensions
{
    public static GetListResponse ToResponse(this core.TodoList list) => new(list.Id, list.Name, list.Items);
}