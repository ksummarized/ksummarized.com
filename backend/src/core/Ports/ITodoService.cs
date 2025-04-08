namespace core.Ports;

public interface ITodoService
{
    Task<TodoList> CreateList(Guid user, string name);
    IEnumerable<TodoList> GetLists(Guid user);
    TodoList? GetList(GetListOptions options);
    bool DeleteList(Guid user, int id);
    Task<bool> RenameList(Guid user, int id, string name);
    Task<TodoItem> CreateItem(Guid user, TodoItem item);
    Task<TodoItem?> GetItem(Guid user, int id);
    IEnumerable<TodoItem> ListItems(Guid user, int? tag, bool? completed);
    Task<bool> DeleteItem(Guid user, int id);
    Task<bool> UpdateItem(Guid user, TodoItem item);

    // Tag related methods
    Task<Tag> CreateTag(Guid user, string name);
    Task<Tag?> GetTag(Guid user, int id);
    IEnumerable<Tag> ListTags(Guid user);
    Task<bool> UpdateTag(Guid user, int id, string name);
    Task<bool> DeleteTag(Guid user, int id);
}
