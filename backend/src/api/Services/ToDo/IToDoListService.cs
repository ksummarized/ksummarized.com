using api.Data.DTO.ToDo;

namespace api.Services.ToDo;

public interface IToDoListService
{
    Task<IEnumerable<ToDoList>> GetToDoLists(string userId);
    Task<ToDoList> CreateList(string userId, string name);
    Task EditList(Guid id, string newName);
    Task DeleteList(Guid id);
}
