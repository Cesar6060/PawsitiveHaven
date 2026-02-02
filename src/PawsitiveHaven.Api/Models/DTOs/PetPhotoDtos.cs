namespace PawsitiveHaven.Api.Models.DTOs;

public record PetPhotoDto(
    int Id,
    int PetId,
    string FileName,
    string FilePath,
    string Url,
    bool IsPrimary,
    DateTime UploadedAt,
    int? UploadedBy
);

public record UploadPhotoResponse(
    bool Success,
    string? Message,
    PetPhotoDto? Photo
);

public record SetPrimaryPhotoResponse(
    bool Success,
    string? Message
);
