namespace PawsitiveHaven.Api.Models.DTOs;

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

public record UpdateEscalationRequest(
    string? Status,
    string? StaffNotes
);

public record EscalationListResponse(
    List<EscalationResponse> Escalations,
    int TotalCount,
    int Page,
    int PageSize
);
