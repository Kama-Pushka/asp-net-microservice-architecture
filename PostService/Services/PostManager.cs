using Domain.Entities;
using Domain.Interfaces;
using Services.Interfaces;

namespace Services;

public class PostManager : IPostManager
{
    private readonly IStorePost _postRepository;

    public PostManager(IStorePost postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Post> GetPostByIdAsync(Guid id)
    {
        return await _postRepository.GetPostByIdAsync(id);
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _postRepository.GetAllPostsAsync();
    }

    public async Task AddPostAsync(Post post)
    {
        await _postRepository.AddPostAsync(post);
    }

    public async Task UpdatePostAsync(Post post)
    {
        await _postRepository.UpdatePostAsync(post);
    }

    public async Task DeletePostAsync(Guid id)
    {
        await _postRepository.DeletePostAsync(id);
    }
}