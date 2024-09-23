using Microsoft.EntityFrameworkCore;
using core.Ports;
using core;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public async Task<TodoItem> CreateItem(string user, TodoItem item)
    {
        var newItem = new TodoItemModel()
        {
            Name = item.Name,
            Owner = Guid.Parse(user),
            Compleated = false,
            Deadline = item.Deadline,
            Notes = item.Notes,
            Subtasks = [],
            Tags = []
        };
        foreach (var st in item.Subtasks)
        {
            var newSubtask = new TodoItemModel()
            {
                Name = st.Name,
                Owner = Guid.Parse(user),
                Compleated = false,
                Deadline = st.Deadline,
                Notes = st.Notes,
                Tags = [],
                Subtasks = []
            };
            newItem.Subtasks.Add(newSubtask);
        }
        foreach (var tag in item.Tags)
        {
            var t = _context.Tags.FirstOrDefault(t => t.Id == tag.Id && t.Owner.Equals(Guid.Parse(user)));
            if (t is null)
            {
                newItem.Tags.Add(new() { Id = tag.Id, Name = tag.Name, Owner = Guid.Parse(user) });
            }
            else
            {
                t.Name = tag.Name;
            }
        }
        await _context.Todos.AddAsync(newItem);
        await _context.SaveChangesAsync();
        return item with { Id = newItem.Id };
    }

    public async Task<TodoItem?> GetItem(string user, int id)
    {
        var item = await _context.Todos
                            .AsNoTracking()
                            .Include(i => i.Subtasks)
                            .Include(i => i.Tags)
                            .AsSplitQuery()
                            .SingleOrDefaultAsync(i => i.Owner.Equals(Guid.Parse(user)) && i.Id == id);
        if (item is null) { return null; }

        return MapTodoItem(item);
    }

    public IEnumerable<TodoItem> ListItems(string user)
    {
        return _context.Todos
                    .AsNoTracking()
                    .Include(i => i.Subtasks)
                    .Include(i => i.Tags)
                    .Where(i => i.Owner.Equals(Guid.Parse(user)) && i.MainTaskId == null)
                    .AsSplitQuery()
                    .Select(i => MapTodoItem(i))
                    .AsEnumerable();
    }

    public async Task<bool> DeleteItem(string user, int id)
    {
        var item = _context.Todos
                                .Include(i => i.Subtasks)
                                .SingleOrDefault(i => i.Owner.Equals(Guid.Parse(user)) && i.Id == id);
        if (item is null) { return false; }
        if (item.Subtasks.Any())
        {
            _context.Todos.RemoveRange(item.Subtasks);
        }
        _context.Todos.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateItem(string user, TodoItem item)
    {
        var existingItem = _context.Todos
                                .Include(i => i.Subtasks)
                                .Include(i => i.Tags)
                                .AsSplitQuery()
                                .SingleOrDefault(i => i.Owner.Equals(Guid.Parse(user)) && i.Id == item.Id);
        if (existingItem is null) { return false; }
        existingItem.Name = item.Name;
        existingItem.Deadline = item.Deadline;
        existingItem.Notes = item.Notes;
        existingItem.Compleated = item.Compleated;
        foreach (var st in item.Subtasks)
        {
            var existingSubtask = existingItem.Subtasks.FirstOrDefault(st => st.Id == st.Id);
            if (existingSubtask is null)
            {
                var newSubtask = new TodoItemModel()
                {
                    Name = st.Name,
                    Owner = Guid.Parse(user),
                    Compleated = st.Compleated,
                    Deadline = st.Deadline,
                    Notes = st.Notes,
                    Tags = [],
                    Subtasks = []
                };
                existingItem.Subtasks.Add(newSubtask);
            }
            else
            {
                existingSubtask.Name = st.Name;
                existingSubtask.Deadline = st.Deadline;
                existingSubtask.Notes = st.Notes;
                existingSubtask.Compleated = st.Compleated;
            }
        }
        var existingTags = existingItem.Tags.ToList();
        foreach (var tag in item.Tags)
        {
            var t = existingTags.FirstOrDefault(t => t.Id == tag.Id);
            if (t is null)
            {
                existingTags.Add(new() { Id = tag.Id, Name = tag.Name, Owner = Guid.Parse(user) });
            }
            else
            {
                t.Name = tag.Name;
            }
        }
        existingItem.Tags = existingTags;

        await _context.SaveChangesAsync();
        return true;
    }

    private static TodoItem MapTodoItem(infrastructure.Data.TodoItemModel item)
    {
        Log.Information("Mapped {item}", JsonSerializer.Serialize(item, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve }));
        return new TodoItem()
        {
            Id = item.Id,
            Name = item.Name,
            Deadline = item.Deadline,
            Notes = item.Notes,
            Subtasks = item.Subtasks?.Select(st => MapSubtask(st)).ToList() ?? [],
            Tags = item.Tags?.Select(t => new core.Tag() { Id = t.Id, Name = t.Name }).ToList() ?? []
        };
    }

    private static TodoItem MapSubtask(infrastructure.Data.TodoItemModel subtask)
    {
        return new TodoItem()
        {
            Id = subtask.Id,
            Name = subtask.Name,
            Deadline = subtask.Deadline,
            Notes = subtask.Notes,
            Subtasks = [],
            Tags = subtask.Tags?.Select(t => new core.Tag() { Id = t.Id, Name = t.Name }).ToList() ?? []
        };
    }
}
