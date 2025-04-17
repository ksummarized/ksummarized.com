namespace core.Ports;

public interface IItemService
{
    Task<TodoItem> CreateItem(Guid user, TodoItem item);
    Task<TodoItem?> GetItem(Guid user, int id);
    IEnumerable<TodoItem> ListItems(Guid user, int? tag, bool? completed);
    Task<bool> DeleteItem(Guid user, int id);
    Task<bool> UpdateItem(Guid user, TodoItem item);
}
