using core;

namespace contracts.Responses;
public record GetListResponse(int Id, string Name, IEnumerable<TodoItem> Items);