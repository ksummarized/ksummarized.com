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

    public IEnumerable<TodoList> GetLists(Guid user)
    {
        return _context.TodoLists.AsNoTracking()
                                 .Where(list => list.Owner.Equals(user))
                                 .Select(list => new TodoList
                                 {
                                     Id = list.Id,
                                     Name = list.Name,
                                     Owner = list.Owner,
                                     Items = Enumerable.Empty<TodoItem>()
                                 })
                                 .AsEnumerable();
    }

    public TodoList? GetList(Guid user, int id)
    {
        var list = _context.TodoLists.AsNoTracking()
                                 .Include(l => l.Items)
                                 .SingleOrDefault(l => l.Owner.Equals(user) && l.Id == id);
        if (list is not null)
        {
            return new()
            {
                Id = list.Id,
                Name = list.Name,
                Owner = list.Owner,
                Items = list.Items.Select(i => MapTodoItem(i)).ToList()
            };
        };
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
        var list = _context.TodoLists
                             .SingleOrDefault(l => l.Owner.Equals(user) && l.Id == id);
        if (list is null) { return false; }
        _context.TodoLists.Remove(list);
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> RenameList(Guid user, int id, string name)
    {
        var list = _context.TodoLists
                         .SingleOrDefault(l => l.Owner.Equals(user) && l.Id == id);

        if (list is null) { return false; }
        list.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TodoItem> CreateItem(Guid user, TodoItem item)
    {
        var newItem = new TodoItemModel()
        {
            Name = item.Name,
            Owner = user,
            Compleated = false,
            Deadline = item.Deadline,
            Notes = item.Notes,
            Subtasks = [],
            Tags = [],
            ListId = item.ListId
        };
        foreach (var st in item.Subtasks)
        {
            var newSubtask = new TodoItemModel()
            {
                Name = st.Name,
                Owner = user,
                Compleated = false,
                Deadline = st.Deadline,
                Notes = st.Notes,
                Tags = [],
                Subtasks = [],
                ListId = item.ListId
            };
            newItem.Subtasks.Add(newSubtask);
        }
        foreach (var tag in item.Tags)
        {
            var t = _context.Tags.FirstOrDefault(t => t.Id == tag.Id && t.Owner.Equals(user));
            if (t is null)
            {
                newItem.Tags.Add(new() { Id = tag.Id, Name = tag.Name, Owner = user });
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

    public async Task<TodoItem?> GetItem(Guid user, int id)
    {
        var item = await _context.Todos
                            .AsNoTracking()
                            .Include(i => i.Subtasks)
                            .Include(i => i.Tags)
                            .AsSplitQuery()
                            .SingleOrDefaultAsync(i => i.Owner.Equals(user) && i.Id == id);
        if (item is null) { return null; }

        return MapTodoItem(item);
    }

    public IEnumerable<TodoItem> ListItems(Guid user)
    {
        return _context.Todos
                    .AsNoTracking()
                    .Include(i => i.Subtasks)
                    .Include(i => i.Tags)
                    .Where(i => i.Owner.Equals(user) && i.MainTaskId == null)
                    .AsSplitQuery()
                    .Select(i => MapTodoItem(i))
                    .AsEnumerable();
    }

    public async Task<bool> DeleteItem(Guid user, int id)
    {
        var item = _context.Todos
                                .Include(i => i.Subtasks)
                                .SingleOrDefault(i => i.Owner.Equals(user) && i.Id == id);
        if (item is null) { return false; }
        if (item.Subtasks.Any())
        {
            _context.Todos.RemoveRange(item.Subtasks);
        }
        _context.Todos.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateItem(Guid user, TodoItem item)
    {
        var existingItem = _context.Todos
                                .Include(i => i.Subtasks)
                                .Include(i => i.Tags)
                                .AsSplitQuery()
                                .SingleOrDefault(i => i.Owner.Equals(user) && i.Id == item.Id);
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
                    Owner = user,
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
                existingTags.Add(new() { Id = tag.Id, Name = tag.Name, Owner = user });
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
        Log.Debug("Mapped {item}", JsonSerializer.Serialize(item, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve }));
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
