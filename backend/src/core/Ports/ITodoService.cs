namespace core.Ports;

public interface ITodoService
{
    Task<TodoList> CreateList(string user, string name);
    IEnumerable<TodoList> GetLists(string userId);
    TodoList? GetList(string userId, int id);
    bool DeleteList(string userId, int id);
    Task<bool> RenameList(string user, int id, string name);
    Task<TodoItem> CreateItem(string user, TodoItem item);
    Task<TodoItem?> GetItem(string user, int id);
    IEnumerable<TodoItem> ListItems(string user);
    Task<bool> DeleteItem(string user, int id);
    Task<bool> UpdateItem(string user, TodoItem item);
}
