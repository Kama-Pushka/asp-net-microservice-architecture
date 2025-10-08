using Dal.Models;

namespace Dal.Interfaces;

public interface IRoleRepository
{
    Task<RoleDal> GetByIdAsync(Guid id);
    Task<IEnumerable<RoleDal>> GetAllAsync();
    Task AddAsync(RoleDal role);
    Task UpdateAsync(RoleDal role);
    Task DeleteAsync(Guid id);
}