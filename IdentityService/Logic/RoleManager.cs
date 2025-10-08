using Dal.Interfaces;
using Dal.Models;
using Logic.Dtos;
using Logic.Interfaces;

namespace Logic;

public class RoleManager : IRoleManager
{
    private readonly IRoleRepository _roleRepository;

    public RoleManager(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleLogic> GetRoleByIdAsync(Guid id)
    {
        var roleDal = await _roleRepository.GetByIdAsync(id);
        return MapToRoleLogic(roleDal);
    }

    public async Task<IEnumerable<RoleLogic>> GetAllRolesAsync()
    {
        var rolesDal = await _roleRepository.GetAllAsync();
        return rolesDal.Select(MapToRoleLogic);
    }

    public async Task AddRoleAsync(RoleLogic role)
    {
        var roleDal = MapToRoleDal(role);
        await _roleRepository.AddAsync(roleDal);
    }

    public async Task UpdateRoleAsync(RoleLogic role)
    {
        var roleDal = MapToRoleDal(role);
        await _roleRepository.UpdateAsync(roleDal);
    }

    public async Task DeleteRoleAsync(Guid id)
    {
        await _roleRepository.DeleteAsync(id);
    }

    private RoleLogic MapToRoleLogic(RoleDal roleDal)
    {
        return new RoleLogic
        {
            Id = roleDal.Id,
            Name = roleDal.Name
        };
    }

    private RoleDal MapToRoleDal(RoleLogic role)
    {
        return new RoleDal
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}