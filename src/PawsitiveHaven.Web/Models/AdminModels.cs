namespace PawsitiveHaven.Web.Models;

// Dashboard
public record DashboardStats(
    int TotalFosters,
    int TotalPets,
    int PendingEscalations,
    int ActiveUsers,
    List<EscalationSummary> RecentEscalations,
    List<UserSummary> RecentUsers
);

public record EscalationSummary(
    int Id,
    string UserName,
    string QuestionPreview,
    string Status,
    DateTime CreatedAt
);

public record UserSummary(
    int Id,
    string Username,
    string UserLevel,
    DateTime CreatedAt
);

// Users
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
