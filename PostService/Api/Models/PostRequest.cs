namespace Api.Models;

public class PostRequest
{
    public required Guid UserId { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
}