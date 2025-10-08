using Dal;
using Dal.Interfaces;
using Dal.Models;
using Logic.Dtos;
using Logic.Interfaces;

namespace Logic;

public class UserRoleManager : IUserRoleManager
{
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleManager(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<RoleLogic>> GetRolesForUserAsync(Guid userId)
    {
        var roles = await _userRoleRepository.GetRolesForUserAsync(userId);
        return roles.Select(r => MapToRoleLogic(r));
    }

    public async Task AddUserRoleAsync(UserRoleLogic userRole)
    {
        await _userRoleRepository.AddUserRoleAsync(MapToUserRoleDal(userRole)); // TODO может лучше userRole.UserId, userRole.RoleId ???
    }

    public async Task DeleteUserRoleAsync(UserRoleLogic userRole) // TODO убрать UserId из тела запроса
    {
        await _userRoleRepository.DeleteUserRoleAsync(userRole.UserId, userRole.RoleId);
    }
    
    private RoleLogic MapToRoleLogic(RoleDal roleDal)
    {
        return new RoleLogic
        {
            Id = roleDal.Id,
            Name = roleDal.Name
        };
    }
    
    private UserRoleDal MapToUserRoleDal(UserRoleLogic userRole)
    {
        return new UserRoleDal
        {
            UserId = userRole.UserId,
            RoleId = userRole.RoleId
        };
    }
}