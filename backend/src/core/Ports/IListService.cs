namespace core.Ports;

public interface IListService
{
    Task<TodoList> CreateList(Guid user, string name);
    IEnumerable<TodoList> GetLists(Guid user);
    TodoList? GetList(GetListOptions options);
    bool DeleteList(Guid user, int id);
    Task<bool> RenameList(Guid user, int id, string name);
}
