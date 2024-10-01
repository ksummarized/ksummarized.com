namespace core.Ports;

public interface ITodoService
{
    Task<TodoList> CreateList(Guid user, string name);
    IEnumerable<TodoList> GetLists(Guid user);
    TodoList? GetList(Guid user, int id);
    bool DeleteList(Guid user, int id);
    Task<bool> RenameList(Guid user, int id, string name);
    Task<TodoItem> CreateItem(Guid user, TodoItem item);
    Task<TodoItem?> GetItem(Guid user, int id);
    IEnumerable<TodoItem> ListItems(Guid user);
    Task<bool> DeleteItem(Guid user, int id);
    Task<bool> UpdateItem(Guid user, TodoItem item);
}
