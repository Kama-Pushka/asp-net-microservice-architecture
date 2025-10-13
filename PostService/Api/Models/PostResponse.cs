namespace Api.Models;

public class PostResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    
}