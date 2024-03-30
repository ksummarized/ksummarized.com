using api.Data.DTO;

namespace api.Services;

public interface ITodoService
{
    public IEnumerable<TodoListDTO> GetLists(string userId);
}