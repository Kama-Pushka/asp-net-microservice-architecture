using Api.Dtos;
using Logic.Dtos;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleManager _userRoleService;

    public UserRoleController(IUserRoleManager userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> GetUserRoles(Guid id)
    {
        var userRoles = await _userRoleService.GetRolesForUserAsync(id);
        return Ok(userRoles.Select(r => MapToRoleResponse(r))); // .ToArray()
    }

    [HttpPost]
    public async Task<IActionResult> AddUserRole(UserRoleRequest userRoleRequest)
    {
        var userRole = MapToUserRoleLogic(userRoleRequest);
        await _userRoleService.AddUserRoleAsync(userRole);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserRole(UserRoleRequest userRoleRequest)
    {
        var userRole = MapToUserRoleLogic(userRoleRequest);
        await _userRoleService.DeleteUserRoleAsync(userRole);
        return NoContent();
    }

    private RoleResponse MapToRoleResponse(RoleLogic userRole)
    {
        return new RoleResponse
        {
            Id = userRole.Id,
            Name = userRole.Name
        };
    }

    private UserRoleLogic MapToUserRoleLogic(UserRoleRequest userRoleRequest)
    {
        return new UserRoleLogic
        {
            UserId = userRoleRequest.UserId,
            RoleId = userRoleRequest.RoleId
        };
    }
}