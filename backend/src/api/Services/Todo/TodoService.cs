using api.Data;
using api.Data.DAO;
using api.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class TodoService : ITodoService
{

    private readonly ApplicationDbContext _context;

    public TodoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TodoListDTO> GetLists(string userId)
    {
        return _context.TodoLists.AsNoTracking()
                                 .Where(list => list.Owner.Equals(Guid.Parse(userId)))
                                 .Select(list => new TodoListDTO(list.Id, list.Name))
                                 .AsEnumerable();
    }

    public TodoListDTO? GetList(string userId, int id)
    {
        var list = _context.TodoLists.AsNoTracking()
                                 .SingleOrDefault(l => l.Owner.Equals(Guid.Parse(userId)) && l.Id == id);
        if (list is not null) { return new(list.Id, list.Name); }
        return null;
    }

    public async Task<TodoListModel> CreateList(string user, string name)
    {
        var newList = new TodoListModel() { Name = name, Owner = Guid.Parse(user) };
        _context.TodoLists.Add(newList);
        await _context.SaveChangesAsync();
        return newList;
    }
}
