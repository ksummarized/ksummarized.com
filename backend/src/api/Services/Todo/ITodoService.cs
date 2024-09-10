using api.Data.DAO;
using api.Data.DTO;

namespace api.Services;

public interface ITodoService
{
    Task<TodoListModel> CreateList(string user, string name);
    public IEnumerable<TodoListDTO> GetLists(string userId);
}