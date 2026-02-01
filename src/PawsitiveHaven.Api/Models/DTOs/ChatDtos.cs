namespace PawsitiveHaven.Api.Models.DTOs;

public record ChatRequest(
    string Message,
    int? ConversationId
);

public record ChatResponse(
    string Response,
    int ConversationId
);

public record ConversationDto(
    int Id,
    string? Title,
    DateTime CreatedAt,
    List<MessageDto> Messages
);

public record MessageDto(
    int Id,
    string Role,
    string Content,
    DateTime CreatedAt
);

public record GenerateBioRequest(
    string Name,
    string Species,
    string? Breed,
    int? Age,
    string? Sex,
    string? AdditionalInfo
);

public record GenerateBioResponse(
    string Bio
);
