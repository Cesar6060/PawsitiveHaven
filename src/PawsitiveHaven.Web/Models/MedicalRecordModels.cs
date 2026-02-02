namespace PawsitiveHaven.Web.Models;

public record MedicalRecordDto(
    int Id,
    int PetId,
    string? PetName,
    string RecordType,
    string Title,
    string? Description,
    DateOnly RecordDate,
    DateOnly? NextDueDate,
    string? Veterinarian,
    string? ClinicName,
    decimal? Cost,
    string? Notes,
    DateTime CreatedAt,
    int? CreatedBy,
    string? CreatedByUsername
);

public record CreateMedicalRecordRequest(
    string RecordType,
    string Title,
    string? Description,
    DateOnly RecordDate,
    DateOnly? NextDueDate,
    string? Veterinarian,
    string? ClinicName,
    decimal? Cost,
    string? Notes
);

public record UpdateMedicalRecordRequest(
    string? RecordType,
    string? Title,
    string? Description,
    DateOnly? RecordDate,
    DateOnly? NextDueDate,
    string? Veterinarian,
    string? ClinicName,
    decimal? Cost,
    string? Notes
);

public record UpcomingMedicalRecordDto(
    int Id,
    int PetId,
    string PetName,
    string RecordType,
    string Title,
    DateOnly NextDueDate,
    int DaysUntilDue
);
