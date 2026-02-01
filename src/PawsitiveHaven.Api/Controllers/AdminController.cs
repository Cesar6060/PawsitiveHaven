using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        ILogger<AdminController> logger)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    // GET: api/admin/users
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(u => new UserDto(
            u.Id,
            u.Username,
            u.Email,
            u.UserLevel,
            u.IsActive,
            u.CreatedAt
        )).ToList();
    }

    // GET: api/admin/users/{id}
    [HttpGet("users/{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.UserLevel,
            user.IsActive,
            user.CreatedAt
        );
    }

    // POST: api/admin/users
    [HttpPost("users")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        // Check if username or email already exists
        var existingUser = await _userRepo.GetByUsernameAsync(request.Username);
        if (existingUser != null)
            return BadRequest("Username already exists");

        existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return BadRequest("Email already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            UserLevel = request.UserLevel ?? "User",
            IsActive = true
        };

        await _userRepo.AddAsync(user);

        _logger.LogInformation("Admin created new user: {Username}", user.Username);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.UserLevel,
            user.IsActive,
            user.CreatedAt
        ));
    }

    // PUT: api/admin/users/{id}
    [HttpPut("users/{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        // Check for username conflicts
        if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
        {
            var existingUser = await _userRepo.GetByUsernameAsync(request.Username);
            if (existingUser != null)
                return BadRequest("Username already exists");
            user.Username = request.Username;
        }

        // Check for email conflicts
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Email already exists");
            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        }

        if (!string.IsNullOrEmpty(request.UserLevel))
        {
            user.UserLevel = request.UserLevel;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        await _userRepo.UpdateAsync(user);

        _logger.LogInformation("Admin updated user: {Username}", user.Username);

        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.UserLevel,
            user.IsActive,
            user.CreatedAt
        );
    }

    // DELETE: api/admin/users/{id}
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        // Prevent deleting yourself
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (user.Id == currentUserId)
            return BadRequest("Cannot delete your own account");

        await _userRepo.DeleteAsync(user);

        _logger.LogInformation("Admin deleted user: {Username}", user.Username);

        return NoContent();
    }
}

public record UserDto(
    int Id,
    string Username,
    string Email,
    string UserLevel,
    bool IsActive,
    DateTime CreatedAt
);

public record UpdateUserRequest(
    string? Username,
    string? Email,
    string? Password,
    string? UserLevel,
    bool? IsActive
);
