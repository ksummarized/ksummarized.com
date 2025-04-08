using Microsoft.EntityFrameworkCore;
using core.Ports;
using core;

namespace infrastructure.Data;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _context;

    public TagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tag> CreateTag(Guid user, string name)
    {
        var newTag = new TagModel { Name = name, Owner = user };
        await _context.Tags.AddAsync(newTag);
        await _context.SaveChangesAsync();
        return new Tag { Id = newTag.Id, Name = newTag.Name };
    }

    public async Task<Tag?> GetTag(Guid user, int id)
    {
        var tag = await _context.Tags
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Owner.Equals(user) && t.Id == id);
        
        return tag is null ? null : new Tag { Id = tag.Id, Name = tag.Name };
    }

    public IEnumerable<Tag> ListTags(Guid user)
    {
        return _context.Tags
            .AsNoTracking()
            .Where(t => t.Owner.Equals(user))
            .Select(t => new Tag { Id = t.Id, Name = t.Name })
            .AsEnumerable();
    }

    public async Task<bool> UpdateTag(Guid user, int id, string name)
    {
        var tag = await _context.Tags
            .SingleOrDefaultAsync(t => t.Owner.Equals(user) && t.Id == id);
        
        if (tag is null) return false;
        
        tag.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTag(Guid user, int id)
    {
        var tag = await _context.Tags
            .SingleOrDefaultAsync(t => t.Owner.Equals(user) && t.Id == id);
        
        if (tag is null) return false;

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }
}
