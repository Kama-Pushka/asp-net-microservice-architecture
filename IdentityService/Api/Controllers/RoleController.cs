using Api.Dtos;
using Logic.Dtos;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleManager _roleService;

    public RoleController(IRoleManager roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleResponse>> GetRole(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(MapToRoleResponse(role));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles.Select(MapToRoleResponse));
    }

    [HttpPost]
    public async Task<ActionResult<RoleResponse>> CreateRole(RoleRequest roleRequest)
    {
        var role = MapToRoleLogic(roleRequest);
        await _roleService.AddRoleAsync(role);
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, MapToRoleResponse(role));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, RoleRequest roleRequest)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return BadRequest();
        }
        
        var roleInfo = MapToRoleLogic(roleRequest);
        await _roleService.UpdateRoleAsync(roleInfo);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }

    private RoleResponse MapToRoleResponse(RoleLogic role)
    {
        return new RoleResponse
        {
            Id = role.Id,
            Name = role.Name
        };
    }

    private RoleLogic MapToRoleLogic(RoleRequest roleRequest)
    {
        return new RoleLogic
        {
            Name = roleRequest.Name
        };
    }
}