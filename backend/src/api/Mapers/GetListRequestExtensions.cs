using api.Endpoints.Lists;
using core;

namespace api.Mapers;

public static class GetListRequestExtensions
{
    public static GetListOptions ToGetListOptions(this Endpoints.Lists.GetListEndpoint.GetListRequest request, int Id, Guid userId)
    {
        return new GetListOptions(
            UserId: userId,
            ListId: Id,
            Tag: request.Tag,
            Completed: request.Compleated,
            Page: request.Page.HasValue ? request.Page!.Value : GetListEndpoint.GetListRequest.DefaultPage,
            PageSize: request.PageSize.HasValue ? request.PageSize!.Value : GetListEndpoint.GetListRequest.DefaultPageSize,
            IncludeSubtasks: !request.IncludeSubtasks.HasValue || request.IncludeSubtasks!.Value
        );
    }
}