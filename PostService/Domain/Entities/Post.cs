using Domain.Interfaces;

namespace Domain.Entities;

public partial record Post // : BaseEntityModel<Guid>
{
    public Guid Id { get; protected set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public required UserInfo UserInfo { get; set; }
}

public record UserInfo(Guid Id, string Name);

public partial record Post // : BaseEntityModel<Guid>
{
    public async Task<Guid> SaveAsync(
        ICheckUser checkUser,
        IStorePost storePost)
    {
        var exists = await checkUser.CheckUserExistAsync(UserInfo.Id);
        if (!exists) throw new InvalidOperationException($"User with id {UserInfo.Id} not found in ProfileService");
        
        Id = await storePost.AddPostAsync(this);
        return Id;
    }
}