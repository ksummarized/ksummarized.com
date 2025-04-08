using core;

namespace infrastructure.Data;

public static class TodoItemMapper
{
    public static TodoItem MapTodoItem(TodoItemModel item, bool includeSubtasks)
    {
        return new TodoItem()
        {
            Id = item.Id,
            Name = item.Name,
            Deadline = item.Deadline,
            Notes = item.Notes,
            Subtasks = includeSubtasks ? (item.Subtasks?.Select(st => MapSubtask(st)).ToList() ?? []) : [],
            Tags = item.Tags.Select(t => new Tag() { Id = t.Id, Name = t.Name }).ToList() ?? [],
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
            Tags = subtask.Tags?.Select(t => new Tag() { Id = t.Id, Name = t.Name }).ToList() ?? [],
            ListId = subtask.ListId,
            Completed = subtask.Completed
        };
    }
}
