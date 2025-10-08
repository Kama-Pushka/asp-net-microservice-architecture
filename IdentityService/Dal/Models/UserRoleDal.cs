namespace Dal.Models;

public class UserRoleDal // : IBaseEntityDal<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}