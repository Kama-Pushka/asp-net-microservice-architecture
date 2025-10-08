using Api.Dtos;
using Logic;
using Logic.Dtos;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserManager _userService;

    public UserController(IUserManager userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(MapToUserResponse(user));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.Select(MapToUserResponse));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(UserRequest userRequest)
    {
        var user = MapToUserLogic(userRequest);
        await _userService.AddUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapToUserResponse(user));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UserRequest userRequest)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return BadRequest();
        }
        
        var userInfo = MapToUserLogic(userRequest);
        await _userService.UpdateUserAsync(userInfo);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    private UserResponse MapToUserResponse(UserLogic user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    private UserLogic MapToUserLogic(UserRequest userRequest)
    {
        return new UserLogic
        {
            Username = userRequest.Username,
            Email = userRequest.Email,
            // PasswordHash = userRequest.Password
        };
    }
}