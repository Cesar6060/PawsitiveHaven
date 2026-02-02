using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public interface INotificationService
{
    Task<NotificationPreferenceDto?> GetPreferencesAsync(int userId);
    Task<NotificationPreferenceDto?> UpdatePreferencesAsync(int userId, UpdateNotificationPreferencesRequest request);
    Task<bool> SendAppointmentReminderAsync(Appointment appointment, User user);
    Task<bool> SendUpcomingTasksDigestAsync(User user, IEnumerable<Appointment> tasks);
}
