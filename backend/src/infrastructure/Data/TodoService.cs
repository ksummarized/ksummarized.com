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
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { ReferenceHandler = ReferenceHandler.Preserve };

    public TodoService(ApplicationDbContext context)
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
        Log.Debug("Getting list {id} for user {user} with tag {tag} and completed {completed}", 
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

            // Apply pagination
            items = items?.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            return new()
            {
                Id = list.Id,
                Name = list.Name,
                Owner = list.Owner,
                Items = items?.Select(i => MapTodoItem(i, options.IncludeSubtasks)).ToList() ?? []
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
            Completed = false,
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
                Completed = false,
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
        await _context.TodoItems.AddAsync(newItem);
        await _context.SaveChangesAsync();
        return item with { Id = newItem.Id };
    }

    public async Task<TodoItem?> GetItem(Guid user, int id)
    {
        var item = await _context.TodoItems
                            .AsNoTracking()
                            .Include(i => i.Subtasks)
                            .Include(i => i.Tags)
                            .AsSplitQuery()
                            .SingleOrDefaultAsync(i => i.Owner.Equals(user) && i.Id == id);
        if (item is null) { return null; }

        return MapTodoItem(item, true);
    }

    public IEnumerable<TodoItem> ListItems(Guid user, int? tag, bool? compleated)
    {
        var baseQuery = _context.TodoItems
                    .AsNoTracking()
                    .Include(i => i.Subtasks)
                    .Include(i => i.Tags)
                    .Where(i => i.Owner.Equals(user) && i.MainTaskId == null)
                    .AsSplitQuery();

        if (tag is not null)
        {
            baseQuery = baseQuery.Where(i => i.Tags.Any(t => t.Id == tag));
        }
        if (compleated is not null)
        {
            baseQuery = baseQuery.Where(i => i.Completed == compleated);
        }
        return baseQuery.Select(i => MapTodoItem(i, true)).AsEnumerable();;
    }

    public async Task<bool> DeleteItem(Guid user, int id)
    {
        var item = _context.TodoItems
                                .Include(i => i.Subtasks)
                                .SingleOrDefault(i => i.Owner.Equals(user) && i.Id == id);
        if (item is null) { return false; }
        if (item.Subtasks.Any())
        {
            _context.TodoItems.RemoveRange(item.Subtasks);
        }
        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateItem(Guid user, TodoItem item)
    {
        var existingItem = _context.TodoItems
                                .Include(i => i.Subtasks)
                                .Include(i => i.Tags)
                                .AsSplitQuery()
                                .SingleOrDefault(i => i.Owner.Equals(user) && i.Id == item.Id);
        if (existingItem is null) { return false; }
        Log.Debug("Updating item {item}", JsonSerializer.Serialize(item, _jsonSerializerOptions));
        existingItem.Name = item.Name;
        existingItem.Deadline = item.Deadline;
        existingItem.Notes = item.Notes;
        existingItem.Completed = item.Completed;
        existingItem.ListId = item.ListId;
        foreach (var st in item.Subtasks)
        {
            var existingSubtask = existingItem.Subtasks.FirstOrDefault(t => t.Id == st.Id);
            if (existingSubtask is null)
            {
                var newSubtask = new TodoItemModel()
                {
                    Name = st.Name,
                    Owner = user,
                    Completed = st.Completed,
                    Deadline = st.Deadline,
                    Notes = st.Notes,
                    Tags = [],
                    Subtasks = [],
                    ListId = item.ListId
                };
                existingItem.Subtasks.Add(newSubtask);
            }
            else
            {
                existingSubtask.Name = st.Name;
                existingSubtask.Deadline = st.Deadline;
                existingSubtask.Notes = st.Notes;
                existingSubtask.Completed = st.Completed;
                existingSubtask.ListId = item.ListId;
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

    private static TodoItem MapTodoItem(TodoItemModel item, bool includeSubtasks)
    {
        Log.Debug("Mapped {item}", JsonSerializer.Serialize(item, _jsonSerializerOptions));
        return new TodoItem()
        {
            Id = item.Id,
            Name = item.Name,
            Deadline = item.Deadline,
            Notes = item.Notes,
            Subtasks = includeSubtasks ? (item.Subtasks?.Select(st => MapSubtask(st)).ToList() ?? []) : [],
            Tags = item.Tags?.Select(t => new core.Tag() { Id = t.Id, Name = t.Name }).ToList() ?? [],
            ListId = item.ListId,
            Completed = item.Completed
        };
    }

    private static TodoItem MapSubtask(TodoItemModel subtask)
    {
        return new TodoItem()
        {
            Id = subtask.Id,
            Name = subtask.Name,
            Deadline = subtask.Deadline,
            Notes = subtask.Notes,
            Subtasks = [],
            Tags = subtask.Tags?.Select(t => new core.Tag() { Id = t.Id, Name = t.Name }).ToList() ?? [],
            ListId = subtask.ListId,
            Completed = subtask.Completed
        };
    }
}
