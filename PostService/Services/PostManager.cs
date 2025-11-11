using Domain.Entities;
using Domain.Interfaces;
using Services.Interfaces;

namespace Services;

public class PostManager : IPostManager
{
    private readonly IStorePost _postRepository;
    private readonly ICheckUser _checkUser;

    public PostManager(IStorePost postRepository,  ICheckUser checkUser)
    {
        _postRepository = postRepository;
        _checkUser = checkUser;
    }

    public async Task<Post> GetPostByIdAsync(Guid id)
    {
        return await _postRepository.GetPostByIdAsync(id);
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _postRepository.GetAllPostsAsync();
    }

    public async Task<Guid> AddPostAsync(Post post)
    {
        return await post.SaveAsync(_checkUser, _postRepository);
    }

    public async Task UpdatePostAsync(Post post)
    {
        await _postRepository.UpdatePostAsync(post);
    }

    public async Task DeletePostAsync(Guid id)
    {
        await _postRepository.DeletePostAsync(id);
    }
    
    public async Task DeletePostsByUserId(Guid id)
    {
        await _postRepository.DeletePostsByUserIdAsync(id);
    }
    
    public async Task UpdatePostsByUserId(Guid id, string userName)
    {
        await _postRepository.UpdatePostsByUserIdAsync(id, userName);
    }
}