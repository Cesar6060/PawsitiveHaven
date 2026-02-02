namespace PawsitiveHaven.Web.Models;

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
    int? FosterId = null,
    string? FosterName = null,
    DateTime? AssignedAt = null,
    string? AssignmentNotes = null
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
