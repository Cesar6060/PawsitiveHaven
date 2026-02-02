namespace PawsitiveHaven.Web.Models;

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
