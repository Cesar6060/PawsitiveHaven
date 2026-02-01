namespace PawsitiveHaven.Api.Models.DTOs;

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
    string? UserLevel,
    bool? IsActive
);
