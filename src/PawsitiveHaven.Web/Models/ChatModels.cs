namespace PawsitiveHaven.Web.Models;

public record ChatRequest(
    string Message,
    int? ConversationId = null
);

public record ChatResponse(
    bool Success,
    string? Response,
    int? ConversationId,
    string? Error
);

public record ConversationDto(
    int Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<MessageDto>? Messages = null
);

public record MessageDto(
    string Role,
    string Content,
    DateTime CreatedAt
);

public record PetBioRequest(
    string Name,
    string Species,
    string? Breed,
    int? Age,
    string? Sex,
    string? Personality
);

public record PetBioResponse(
    bool Success,
    string? Bio,
    string? Error
);

public record CreateEscalationRequest(
    int ConversationId,
    int? MessageId,
    string UserEmail,
    string UserName,
    string UserQuestion,
    string? AdditionalContext
);

public record EscalationResponse(
    int Id,
    int ConversationId,
    string UserEmail,
    string UserName,
    string UserQuestion,
    string? AdditionalContext,
    string Status,
    DateTime CreatedAt,
    DateTime? EmailSentAt,
    DateTime? ResolvedAt,
    string? StaffNotes
);
