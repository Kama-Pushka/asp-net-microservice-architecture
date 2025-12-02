using Domain.Entities;

namespace Domain.Interfaces;

public interface IStorePost
{
    Task<Post> GetPostByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetAllPostsAsync();
    Task<Guid> AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(Guid id);
    Task DeletePostsByUserIdAsync(Guid id);
    Task UpdatePostsByUserIdAsync(Guid id, string userName);
}