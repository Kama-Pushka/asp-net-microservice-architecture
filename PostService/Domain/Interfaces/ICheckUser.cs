namespace Domain.Interfaces;

public interface ICheckUser
{
    Task<bool> CheckUserExistAsync(Guid userId);
}