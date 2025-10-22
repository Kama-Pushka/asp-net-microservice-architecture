using Dal;
using Dal.Interfaces;
using Dal.Models;
using Logic.Dtos;
using Logic.Interfaces;

namespace Logic;

public class UserManager : IUserManager
{
    private readonly IUserRepository _userRepository;

    public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserLogic> GetUserByIdAsync(Guid id)
    {
        var userDal = await _userRepository.GetByIdAsync(id);
        return userDal == null ? null : MapToUserLogic(userDal);
    }

    public async Task<IEnumerable<UserLogic>> GetAllUsersAsync()
    {
        var usersDal = await _userRepository.GetAllAsync();
        return usersDal.Select(MapToUserLogic);
    }

    public async Task AddUserAsync(UserLogic user)
    {
        var userDal = MapToUserDal(user);
        await _userRepository.AddAsync(userDal);
    }

    public async Task UpdateUserAsync(UserLogic user)
    {
        var userDal = MapToUserDal(user);
        await _userRepository.UpdateAsync(userDal);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }

    private UserLogic MapToUserLogic(UserDal userDal)
    {
        return new UserLogic
        {
            Id = userDal.Id,
            Username = userDal.Username,
            Email = userDal.Email,
            // PasswordHash = userDal.PasswordHash
        };
    }

    private UserDal MapToUserDal(UserLogic user)
    {
        return new UserDal
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            // PasswordHash = user.PasswordHash
        };
    }
}