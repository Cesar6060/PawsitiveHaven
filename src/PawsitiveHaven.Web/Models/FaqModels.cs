namespace PawsitiveHaven.Web.Models;

public record FaqDto(
    int Id,
    string Question,
    string Answer,
    int DisplayOrder,
    bool IsActive
);
