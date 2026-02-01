using System.ComponentModel.DataAnnotations;

namespace PawsitiveHaven.Api.Models.DTOs;

public record UserDto(
    int Id,
    string Username,
    string Email,
    string UserLevel,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateUserRequest(
    [Required, StringLength(50, MinimumLength = 3)] string Username,
    [Required, EmailAddress] string Email,
    [Required, StringLength(100, MinimumLength = 8)] string Password,
    string? UserLevel
);

public record UpdateUserRequest(
    [StringLength(50, MinimumLength = 3)] string? Username,
    [EmailAddress] string? Email,
    [StringLength(100, MinimumLength = 8)] string? Password,
    string? UserLevel,
    bool? IsActive
);
