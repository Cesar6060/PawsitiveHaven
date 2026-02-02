namespace PawsitiveHaven.Api.Models.DTOs;

public record DashboardStatsDto(
    int TotalFosters,
    int TotalPets,
    int PendingEscalations,
    int ActiveUsers,
    List<EscalationSummaryDto> RecentEscalations,
    List<UserSummaryDto> RecentUsers
);

public record EscalationSummaryDto(
    int Id,
    string UserName,
    string QuestionPreview,
    string Status,
    DateTime CreatedAt
);

public record UserSummaryDto(
    int Id,
    string Username,
    string UserLevel,
    DateTime CreatedAt
);
