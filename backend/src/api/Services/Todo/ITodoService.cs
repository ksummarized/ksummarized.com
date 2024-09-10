using api.Data.DAO;
using api.Data.DTO;

namespace api.Services;

public interface ITodoService
{
    Task<TodoListModel> CreateList(string user, string name);
    IEnumerable<TodoListDTO> GetLists(string userId);
    TodoListDTO? GetList(string userId, int id);
    bool DeleteList(string userId, int id);
}
