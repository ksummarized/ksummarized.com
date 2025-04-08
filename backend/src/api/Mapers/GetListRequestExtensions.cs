using core;
using contracts.Requests;

namespace api.Mapers;

public static class GetListRequestExtensions
{
    public static GetListOptions ToGetListOptions(this GetListRequest request, int Id, Guid userId)
    {
        return new GetListOptions(
            UserId: userId,
            ListId: Id,
            Tag: request.Tag,
            Completed: request.Compleated,
            Page: request.Page.HasValue ? request.Page!.Value : GetListRequest.DefaultPage,
            PageSize: request.PageSize.HasValue ? request.PageSize!.Value : GetListRequest.DefaultPageSize,
            IncludeSubtasks: !request.IncludeSubtasks.HasValue || request.IncludeSubtasks!.Value
        );
    }
}