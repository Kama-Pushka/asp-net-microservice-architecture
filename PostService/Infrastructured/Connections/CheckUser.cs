using Domain.Interfaces;
using IdentityConnectionLib.ConnectionServices.Dtos;
using IdentityConnectionLib.ConnectionServices.Interfaces;

namespace Infrastructured.Connections;

internal class CheckUser : ICheckUser
{
    private readonly IIdentityConnectionService _profileConnectionService;
    
    public CheckUser(IIdentityConnectionService profileConnectionService)
    {
        _profileConnectionService =  profileConnectionService;
    }

    public async Task<bool> CheckUserExistAsync(Guid userId)
    {
        var res = await _profileConnectionService.CheckUserExistAsync(new CheckUserExistsIdentityServiceRequest
        {
            UserId = userId
        });
        return res.exists;
    }
}