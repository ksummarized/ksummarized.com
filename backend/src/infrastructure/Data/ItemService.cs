using Microsoft.EntityFrameworkCore;
using core.Ports;
using core;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace infrastructure.Data;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _context;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { ReferenceHandler = ReferenceHandler.Preserve };

    public ItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItem> CreateItem(Guid user, TodoItem item)
    {
        var newItem = new TodoItemModel()
        {
            Name = item.Name,
            Owner = user,
            Completed = item.Completed,
            Deadline = item.Deadline,
            Notes = item.Notes,
            Subtasks = [],
            Tags = [],
            ListId = item.ListId
        };

        foreach (var tag in item.Tags.Select(t => t.Name))
        {
            var t = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag && t.Owner.Equals(user));
            if (t is not null)
            {
                newItem.Tags.Add(t);
            }
            else
            {
                newItem.Tags.Add(new() { Name = tag, Owner = user });
            }
        }

        foreach (var subtask in item.Subtasks)
        {
            var newSubtask = new TodoItemModel()
            {
                Name = subtask.Name,
                Owner = user,
                Completed = subtask.Completed,
                Deadline = subtask.Deadline,
                Notes = subtask.Notes,
                Tags = [],
                Subtasks = [],
                ListId = item.ListId
            };
            foreach (var tag in subtask.Tags.Select(t => t.Name ))
            {
                var t = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag && t.Owner.Equals(user));
                if (t is not null)
                {
                    newSubtask.Tags.Add(t);
                }
                else
                {
                    newSubtask.Tags.Add(new() { Name = tag, Owner = user });
                }
            }
            newItem.Subtasks.Add(newSubtask);
        }

        await _context.TodoItems.AddAsync(newItem);
        await _context.SaveChangesAsync();
        return TodoItemMapper.MapTodoItem(newItem, includeSubtasks: true);
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

        return TodoItemMapper.MapTodoItem(item, true);
    }

    public IEnumerable<TodoItem> ListItems(Guid user, int? tag, bool? completed)
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
        if (completed is not null)
        {
            baseQuery = baseQuery.Where(i => i.Completed == completed);
        }
        return baseQuery.Select(i => TodoItemMapper.MapTodoItem(i, true)).AsEnumerable();
    }

    public async Task<bool> DeleteItem(Guid user, int id)
    {
        var item = await _context.TodoItems
                                .Include(i => i.Subtasks)
                                .SingleOrDefaultAsync(i => i.Owner.Equals(user) && i.Id == id);
        if (item is null) { return false; }
        if (item.Subtasks.Count != 0)
        {
            _context.TodoItems.RemoveRange(item.Subtasks);
        }
        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateItem(Guid user, TodoItem item)
    {
        var existingItem = await _context.TodoItems
                                .Include(i => i.Subtasks)
                                .Include(i => i.Tags)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(i => i.Owner.Equals(user) && i.Id == item.Id);
        if (existingItem is null) { return false; }
        Log.Debug("Updating item {Item}", JsonSerializer.Serialize(item, _jsonSerializerOptions));
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
                // If DTO provides an ID, assume it's for an existing tag. Fetch and add that.
                if (tag.Id != 0)
                {
                    var tagInstanceFromDb = await _context.Tags.FindAsync(tag.Id);
                    // Ensure it's the correct owner and not null before adding
                    if (tagInstanceFromDb != null && tagInstanceFromDb.Owner == user)
                    {
                        if (!existingTags.Contains(tagInstanceFromDb)) // Avoid adding if somehow already there by reference
                        {
                            existingTags.Add(tagInstanceFromDb);
                        }
                    }
                    // Optional: else, handle case where tag ID is provided but not found / wrong owner
                }
                else // DTO tag.Id is 0, implies new tag by name. Create if not already existing by name for this user.
                {
                    var existingTagByName = await _context.Tags.FirstOrDefaultAsync(dbTag => dbTag.Name == tag.Name && dbTag.Owner == user);
                    if (existingTagByName != null)
                    {
                        if (!existingTags.Contains(existingTagByName)) existingTags.Add(existingTagByName);
                    }
                    else
                    {
                        existingTags.Add(new TagModel { Name = tag.Name, Owner = user });
                    }
                }
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
}
