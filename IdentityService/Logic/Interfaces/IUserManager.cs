using Logic.Dtos;

namespace Logic.Interfaces;

public interface IUserManager
{
    Task<UserLogic> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserLogic>> GetAllUsersAsync();
    Task AddUserAsync(UserLogic user);
    Task UpdateUserAsync(UserLogic user);
    Task DeleteUserAsync(Guid id);
}