using Microsoft.EntityFrameworkCore;

namespace api.Data.DAO.ToDo;

[PrimaryKey(nameof(ItemId), nameof(TagId))]
[Index(nameof(TagId), nameof(ItemId), IsUnique = true)]
public class ToDoItemTags
{
    public Guid ItemId { get; set; }
    public int TagId { get; set; }

    public ToDoItem? Item { get; set; }
    public ToDoTag? Tag { get; set; }
}
