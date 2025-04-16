namespace contracts.Requests;
public record CreateListRequest(string Name);

public class GetListRequest : PaginatedRequest
{
    public const bool DefaultIncludeSubtasks = true;

    public int? Tag { get; init; }
    public bool? Completed { get; init; }
    public bool? IncludeSubtasks { get; init; } = DefaultIncludeSubtasks;
}

public class ListRenameRequest
{
    public required string Name { get; set; }
}