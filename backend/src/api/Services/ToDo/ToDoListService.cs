using api.Data;
using api.Data.DTO.ToDo;
using Microsoft.EntityFrameworkCore;

namespace api.Services.ToDo;

public class ToDoListService : IToDoListService
{
    private readonly ApplicationDbContext _dbContext;

    public ToDoListService(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<ToDoList>> GetToDoLists(string userId)
    {
        return (await _dbContext.ToDoLists.AsNoTracking()
                                   .Where(list => list.UserId.Equals(userId))
                                   .ToListAsync())
                                   .Select(list => new ToDoList
                                   {
                                       Id = list.Id,
                                       Name = list.Name
                                   });
    }

    public async Task<ToDoList> CreateList(string userId, string name)
    {
        var list = new Data.DAO.ToDo.ToDoList { Name = name, UserId = userId };
        await _dbContext.ToDoLists.AddAsync(list);
        await _dbContext.SaveChangesAsync();
        return new ToDoList{Id = list.Id, Name = list.Name};
    }

    public async Task EditList(Guid id, string newName)
    {
        _dbContext.ToDoLists.Single(list => list.Id == id).Name = newName;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteList(Guid id)
    {
        var list = await _dbContext.ToDoLists.SingleAsync(list => list.Id == id);
        _dbContext.ToDoLists.Remove(list);
        await _dbContext.SaveChangesAsync();
    }
}
