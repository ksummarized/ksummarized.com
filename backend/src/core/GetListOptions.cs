namespace core;

public record GetListOptions(
    Guid UserId,
    int ListId,
    int? Tag,
    bool? Completed,
    int Page,
    int PageSize,
    bool IncludeSubtasks
); 