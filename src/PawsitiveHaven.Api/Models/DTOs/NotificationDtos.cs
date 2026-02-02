namespace PawsitiveHaven.Api.Models.DTOs;

public record NotificationPreferenceDto(
    int Id,
    int UserId,
    bool EmailAppointments,
    bool EmailReminders,
    int ReminderDaysBefore
);

public record UpdateNotificationPreferencesRequest(
    bool? EmailAppointments,
    bool? EmailReminders,
    int? ReminderDaysBefore
);
