namespace PawsitiveHaven.Api.Models.DTOs;

public record PetDto(
    int Id,
    int UserId,
    string Name,
    string Species,
    string? Breed,
    int? Age,
    string? Sex,
    string? Bio,
    string? ImageUrl,
    int? FosterId,
    string? FosterName,
    DateTime? AssignedAt,
    string? AssignmentNotes
);

public record CreatePetRequest(
    string Name,
    string Species,
    string? Breed,
    int? Age,
    string? Sex,
    string? Bio,
    string? ImageUrl
);

public record UpdatePetRequest(
    string? Name,
    string? Species,
    string? Breed,
    int? Age,
    string? Sex,
    string? Bio,
    string? ImageUrl
);

public record AssignPetRequest(
    int FosterId,
    string? Notes
);

public record PetAssignmentDto(
    int PetId,
    string PetName,
    string Species,
    string? Breed,
    int? FosterId,
    string? FosterName,
    DateTime? AssignedAt,
    string? AssignmentNotes
);
