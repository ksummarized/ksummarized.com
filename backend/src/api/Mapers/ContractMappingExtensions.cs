using contracts.Requests;
using contracts.Responses; 
using core;

namespace api.Mapers;

public static class ContractMappingExtensions {
    public static TodoItem MapToTodoItem(this CreateTaskRequest item) => new()
    {
        ListId = item.ListId,
        Id = null,
        Name = item.Name,
        Completed = false,
        Deadline = item.Deadline,
        Tags = item.Tags.Select(t => new Tag { Name = t }),
        Notes = item.Notes,
        Subtasks = item.Subtasks.Select(s => new TodoItem
        {
            Id = null,
            Name = s.Name,
            Completed = false,
            Deadline = s.Deadline,
            Notes = s.Notes,
            Tags = s.Tags.Select(t => new Tag { Name = t }),
            Subtasks = [],
            ListId = item.ListId
        }),
    };

    public static CreateTaskResponse MapToResponse(this TodoItem item) => new()
    {
        ListId = item.ListId,
        Id = item.Id ?? 0,
        Name = item.Name,
        Completed = item.Completed,
        Deadline = item.Deadline,
        Notes = item.Notes,
        Tags = item.Tags.Select(t => new CreateTaskResponse.Tag { Id = t.Id, Name = t.Name }),
        Subtasks = item.Subtasks.Select(s => new CreateTaskResponse.Subtask
        {
            Id = s.Id ?? 0,
            Name = s.Name,
            Completed = s.Completed,
            Deadline = s.Deadline,
            Notes = s.Notes,
            Tags = s.Tags.Select(t => new CreateTaskResponse.Tag { Id = t.Id, Name = t.Name }),
        })
    };
}