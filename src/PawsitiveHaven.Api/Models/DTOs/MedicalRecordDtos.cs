using System.ComponentModel.DataAnnotations;

namespace PawsitiveHaven.Api.Models.DTOs;

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
    [Required]
    [StringLength(50)]
    string RecordType,

    [Required]
    [StringLength(200)]
    string Title,

    string? Description,

    [Required]
    DateOnly RecordDate,

    DateOnly? NextDueDate,

    [StringLength(100)]
    string? Veterinarian,

    [StringLength(100)]
    string? ClinicName,

    [Range(0, 999999.99)]
    decimal? Cost,

    string? Notes
);

public record UpdateMedicalRecordRequest(
    [StringLength(50)]
    string? RecordType,

    [StringLength(200)]
    string? Title,

    string? Description,

    DateOnly? RecordDate,

    DateOnly? NextDueDate,

    [StringLength(100)]
    string? Veterinarian,

    [StringLength(100)]
    string? ClinicName,

    [Range(0, 999999.99)]
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
