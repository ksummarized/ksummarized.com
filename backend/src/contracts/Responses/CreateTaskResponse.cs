namespace contracts.Responses;

public class CreateTaskResponse
{
    public int ListId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Completed { get; set; }
    public DateTime? Deadline { get; set; }
    public string Notes { get; set; }
    public IEnumerable<Tag> Tags { get; set; }
    public IEnumerable<Subtask> Subtasks { get; set; }

    public class Subtask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }
        public DateTime? Deadline { get; set; }
        public string Notes { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}