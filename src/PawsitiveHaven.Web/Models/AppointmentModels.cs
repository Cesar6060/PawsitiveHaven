namespace PawsitiveHaven.Web.Models;

public record AppointmentDto(
    int Id,
    int UserId,
    int? PetId,
    string? PetName,
    string Title,
    string? Description,
    DateOnly AppointmentDate,
    TimeOnly? AppointmentTime,
    bool IsCompleted
);

public record CreateAppointmentRequest(
    int? PetId,
    string Title,
    string? Description,
    DateOnly AppointmentDate,
    TimeOnly? AppointmentTime
);

public record UpdateAppointmentRequest(
    int? PetId,
    string? Title,
    string? Description,
    DateOnly? AppointmentDate,
    TimeOnly? AppointmentTime,
    bool? IsCompleted
);
