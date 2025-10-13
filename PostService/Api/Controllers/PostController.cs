using Api.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostManager _postService;

    public PostController(IPostManager postService)
    {
        _postService = postService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPost(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post); // TODO PostResponse
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    {
        var posts = await _postService.GetAllPostsAsync();
        // TODO PostListResponse???
        return Ok(posts);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] PostRequest postRequest)
    {
        var post = new Post()
        {
            Id = Guid.NewGuid(), // TODO переместить, тут этого быть не должно
            Title = postRequest.Title,
            Content = postRequest.Content,
            CreatedAt = DateTime.Now
        };
        await _postService.AddPostAsync(post);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, Post post)
    {
        if (id != post.Id)
        {
            return BadRequest();
        }
        await _postService.UpdatePostAsync(post);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        await _postService.DeletePostAsync(id);
        return NoContent();
    }
}