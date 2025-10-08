using Dal.Models;

namespace Dal.Interfaces;

public interface IUserRepository
{
    Task<UserDal> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDal>> GetAllAsync();
    Task AddAsync(UserDal user);
    Task UpdateAsync(UserDal user);
    Task DeleteAsync(Guid id);
}