namespace contracts.Requests;
public class CreateTaskRequest
{
    public required int ListId { get; set; }
    public required string Name { get; set; }
    public DateTime? Deadline { get; set; }
    public string Notes { get; set; } = string.Empty;
    public required IEnumerable<string> Tags { get; set; }
    public required IEnumerable<Subtask> Subtasks { get; set; }

    public class Subtask
    {
        public required string Name { get; set; }
        public DateTime? Deadline { get; set; }
        public string Notes { get; set; } = string.Empty;
        public required IEnumerable<string> Tags { get; set; }
    }
}