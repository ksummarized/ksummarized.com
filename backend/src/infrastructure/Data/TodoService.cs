using Microsoft.EntityFrameworkCore;
using core.Ports;
using core;

namespace infrastructure.Data;

public class TodoService : ITodoService
{

    private readonly ApplicationDbContext _context;

    public TodoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TodoList> GetLists(string userId)
    {
        return _context.TodoLists.AsNoTracking()
                                 .Where(list => list.Owner.Equals(Guid.Parse(userId)))
                                 .Select(list => new TodoList { Id = list.Id, Name = list.Name, Owner = list.Owner })
                                 .AsEnumerable();
    }

    public TodoList? GetList(string userId, int id)
    {
        var list = _context.TodoLists.AsNoTracking()
                                 .SingleOrDefault(l => l.Owner.Equals(Guid.Parse(userId)) && l.Id == id);
        //TODO: Consider creating a mapper instead of this manual new
        if (list is not null) { return new() { Id = list.Id, Name = list.Name, Owner = list.Owner }; }
        return null;
    }

    public async Task<TodoList> CreateList(string user, string name)
    {
        var newList = new TodoListModel() { Name = name, Owner = Guid.Parse(user) };
        _context.TodoLists.Add(newList);
        await _context.SaveChangesAsync();
        return new TodoList() { Id = newList.Id, Name = newList.Name, Owner = newList.Owner };
    }

    public bool DeleteList(string userId, int id)
    {
        var list = _context.TodoLists
                             .SingleOrDefault(l => l.Owner.Equals(Guid.Parse(userId)) && l.Id == id);
        if (list is null) { return false; }
        _context.TodoLists.Remove(list);
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> RenameList(string userId, int id, string name)
    {
        var list = _context.TodoLists
                         .SingleOrDefault(l => l.Owner.Equals(Guid.Parse(userId)) && l.Id == id);

        if (list is null) { return false; }
        list.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }
}
