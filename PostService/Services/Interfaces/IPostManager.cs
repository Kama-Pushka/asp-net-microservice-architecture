using Domain.Entities;

namespace Services.Interfaces;

public interface IPostManager
{
    Task<Post> GetPostByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetAllPostsAsync();
    Task<Guid> AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(Guid id);
}