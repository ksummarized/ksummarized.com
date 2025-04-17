namespace core.Ports;

public interface ITagService
{
    Task<Tag> CreateTag(Guid user, string name);
    Task<Tag?> GetTag(Guid user, int id);
    IEnumerable<Tag> ListTags(Guid user);
    Task<bool> UpdateTag(Guid user, int id, string name);
    Task<bool> DeleteTag(Guid user, int id);
}
