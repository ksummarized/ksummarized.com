namespace core;

public class TodoList
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required Guid Owner { get; set; }

    public bool IsAuthorized(Guid person) => Owner.Equals(person);
}
