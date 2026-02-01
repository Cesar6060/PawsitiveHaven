namespace PawsitiveHaven.Web.Models;

public record UserDto(
    int Id,
    string Username,
    string Email,
    string UserLevel,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string? UserLevel
);

public record UpdateUserRequest(
    string? Username,
    string? Email,
    string? Password,
    string? UserLevel,
    bool? IsActive
);

public record CreateFaqRequest(
    string Question,
    string Answer,
    int DisplayOrder,
    bool IsActive
);

public record UpdateFaqRequest(
    string? Question,
    string? Answer,
    int? DisplayOrder,
    bool? IsActive
);
