namespace api.Data.DTO;

public class GitHubEmailsResponse
{
    public string email { get; set; }
    public bool primary { get; set; }
    public bool verified { get; set; }
    public string visibility { get; set; }
}
