using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructured.Data;

public class PostRepository : IStorePost
{
    private readonly PostDbContext _context;

    public PostRepository(PostDbContext context)
    {
        _context = context;
    }

    public async Task<Post> GetPostByIdAsync(Guid id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.ToListAsync();
    }

    public async Task<Guid> AddPostAsync(Post post)
    {
        var e = _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return e.Entity.Id;
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeletePostsByUserIdAsync(Guid id)
    {
        await _context.Posts
            .Where(p => p.UserInfo.Id == id)
            .ExecuteDeleteAsync();
    }
    
    public async Task UpdatePostsByUserIdAsync(Guid id,  string userName)
    {
        await _context.Posts
            .Where(p => p.UserInfo.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.UserInfo.Name, userName));
    }
}