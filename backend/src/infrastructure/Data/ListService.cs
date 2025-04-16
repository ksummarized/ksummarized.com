using Microsoft.EntityFrameworkCore;
using core.Ports;
using core;
using Serilog;

namespace infrastructure.Data;

public class ListService : IListService
{
    private readonly ApplicationDbContext _context;

    public ListService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TodoList> GetLists(Guid user)
    {
        var empty = Enumerable.Empty<TodoItem>();
        return _context.TodoLists.AsNoTracking()
                                 .Where(list => list.Owner.Equals(user))
                                 .Select(list => new TodoList
                                 {
                                     Id = list.Id,
                                     Name = list.Name,
                                     Owner = list.Owner,
                                     Items = empty
                                 })
                                 .AsEnumerable();
    }

    public TodoList? GetList(GetListOptions options)
    {
        Log.Debug("Getting list {Id} for user {User} with tag {Tag} and completed {Completed}",
            options.ListId, options.UserId, options.Tag, options.Completed);

        var query = _context.TodoLists.AsNoTracking();

        if (options.IncludeSubtasks)
        {
            query = query.Include(l => l.Items)
                        .ThenInclude(i => i.Tags);
        }
        else
        {
            query = query.Include(l => l.Items.Where(i => i.MainTaskId == null))
                        .ThenInclude(i => i.Tags);
        }

        var list = query.SingleOrDefault(l => l.Owner.Equals(options.UserId) && l.Id == options.ListId);

        if (list is not null)
        {
            var items = list.Items.AsQueryable();
            if (options.Tag is not null)
            {
                items = items?.Where(i => i.Tags.Any(t => t.Id == options.Tag));
            }
            if (options.Completed is not null)
            {
                items = items?.Where(i => i.Completed == options.Completed);
            }

            items = items?.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            return new()
            {
                Id = list.Id,
                Name = list.Name,
                Owner = list.Owner,
                Items = items?.Select(i => TodoItemMapper.MapTodoItem(i, options.IncludeSubtasks)).ToList() ?? []
            };
        }
        return null;
    }

    public async Task<TodoList> CreateList(Guid user, string name)
    {
        var newList = new TodoListModel() { Name = name, Owner = user, Items = [] };
        _context.TodoLists.Add(newList);
        await _context.SaveChangesAsync();
        return new TodoList() { Id = newList.Id, Name = newList.Name, Owner = newList.Owner, Items = [] };
    }

    public bool DeleteList(Guid user, int id)
    {
        var list = _context.TodoLists.SingleOrDefault(l => l.Owner.Equals(user) && l.Id == id);
        if (list is null) { return false; }
        _context.TodoLists.Remove(list);
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> RenameList(Guid user, int id, string name)
    {
        var list = await _context.TodoLists.SingleOrDefaultAsync(l => l.Owner.Equals(user) && l.Id == id);
        if (list is null) { return false; }
        list.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }
}
