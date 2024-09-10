namespace core.Ports;

public interface ITodoService
{
    Task<TodoList> CreateList(string user, string name);
    IEnumerable<TodoList> GetLists(string userId);
    TodoList? GetList(string userId, int id);
    bool DeleteList(string userId, int id);
    Task<bool> RenameList(string user, int id, string name);
}
