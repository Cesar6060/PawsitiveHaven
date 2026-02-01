namespace PawsitiveHaven.Api.Models.DTOs;

public record FaqDto(
    int Id,
    string Question,
    string Answer,
    int DisplayOrder,
    bool IsActive
);

public record CreateFaqRequest(
    string Question,
    string Answer,
    int DisplayOrder = 0
);

public record UpdateFaqRequest(
    string? Question,
    string? Answer,
    int? DisplayOrder,
    bool? IsActive
);
