using SendGrid;
using SendGrid.Helpers.Mail;
using PawsitiveHaven.Api.Configuration;
using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly SendGridClient? _client;
    private readonly SendGridConfig _config;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationPreferenceRepository preferenceRepository,
        SendGridConfig config,
        ILogger<NotificationService> logger)
    {
        _preferenceRepository = preferenceRepository;
        _config = config;
        _logger = logger;

        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            _client = new SendGridClient(config.ApiKey);
        }
        else
        {
            _logger.LogWarning("SendGrid API key not configured. Email notifications will be disabled.");
        }
    }

    public async Task<NotificationPreferenceDto?> GetPreferencesAsync(int userId)
    {
        var preferences = await _preferenceRepository.GetByUserIdAsync(userId);

        if (preferences == null)
        {
            // Create default preferences for the user
            preferences = new NotificationPreference
            {
                UserId = userId,
                EmailAppointments = true,
                EmailReminders = true,
                ReminderDaysBefore = 1
            };

            await _preferenceRepository.AddAsync(preferences);
            _logger.LogInformation("Created default notification preferences for user {UserId}", userId);
        }

        return MapToDto(preferences);
    }

    public async Task<NotificationPreferenceDto?> UpdatePreferencesAsync(int userId, UpdateNotificationPreferencesRequest request)
    {
        try
        {
            var preferences = await _preferenceRepository.GetByUserIdAsync(userId);

            if (preferences == null)
            {
                // Create new preferences with requested values
                preferences = new NotificationPreference
                {
                    UserId = userId,
                    EmailAppointments = request.EmailAppointments ?? true,
                    EmailReminders = request.EmailReminders ?? true,
                    ReminderDaysBefore = request.ReminderDaysBefore ?? 1
                };

                await _preferenceRepository.AddAsync(preferences);
                _logger.LogInformation("Created notification preferences for user {UserId}", userId);
            }
            else
            {
                // Update existing preferences
                if (request.EmailAppointments.HasValue)
                    preferences.EmailAppointments = request.EmailAppointments.Value;
                if (request.EmailReminders.HasValue)
                    preferences.EmailReminders = request.EmailReminders.Value;
                if (request.ReminderDaysBefore.HasValue)
                    preferences.ReminderDaysBefore = request.ReminderDaysBefore.Value;

                preferences.UpdatedAt = DateTime.UtcNow;

                await _preferenceRepository.UpdateAsync(preferences);
                _logger.LogInformation("Updated notification preferences for user {UserId}", userId);
            }

            return MapToDto(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences for user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> SendAppointmentReminderAsync(Appointment appointment, User user)
    {
        if (_client == null)
        {
            _logger.LogWarning("Cannot send appointment reminder - SendGrid not configured");
            return false;
        }

        // Check user preferences
        var preferences = await _preferenceRepository.GetByUserIdAsync(user.Id);
        if (preferences != null && !preferences.EmailReminders)
        {
            _logger.LogInformation("User {UserId} has disabled email reminders, skipping", user.Id);
            return true;
        }

        try
        {
            var from = new EmailAddress(_config.FromEmail, _config.FromName);
            var to = new EmailAddress(user.Email, user.Username);
            var subject = $"Reminder: {appointment.Title} - {appointment.AppointmentDate:MMMM dd, yyyy}";

            var htmlContent = BuildAppointmentReminderHtml(appointment, user);
            var plainTextContent = BuildAppointmentReminderPlainText(appointment, user);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await _client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Appointment reminder sent successfully for appointment {AppointmentId} to user {UserId}",
                    appointment.Id, user.Id);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send appointment reminder. Status: {StatusCode}, Body: {Body}",
                    response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending appointment reminder for appointment {AppointmentId}", appointment.Id);
            return false;
        }
    }

    public async Task<bool> SendUpcomingTasksDigestAsync(User user, IEnumerable<Appointment> tasks)
    {
        if (_client == null)
        {
            _logger.LogWarning("Cannot send tasks digest - SendGrid not configured");
            return false;
        }

        var taskList = tasks.ToList();
        if (!taskList.Any())
        {
            _logger.LogInformation("No upcoming tasks for user {UserId}, skipping digest email", user.Id);
            return true;
        }

        // Check user preferences
        var preferences = await _preferenceRepository.GetByUserIdAsync(user.Id);
        if (preferences != null && !preferences.EmailAppointments)
        {
            _logger.LogInformation("User {UserId} has disabled appointment emails, skipping digest", user.Id);
            return true;
        }

        try
        {
            var from = new EmailAddress(_config.FromEmail, _config.FromName);
            var to = new EmailAddress(user.Email, user.Username);
            var subject = $"Your Upcoming Tasks - {taskList.Count} item{(taskList.Count != 1 ? "s" : "")}";

            var htmlContent = BuildTasksDigestHtml(taskList, user);
            var plainTextContent = BuildTasksDigestPlainText(taskList, user);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await _client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Tasks digest sent successfully to user {UserId} with {TaskCount} tasks",
                    user.Id, taskList.Count);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send tasks digest. Status: {StatusCode}, Body: {Body}",
                    response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending tasks digest for user {UserId}", user.Id);
            return false;
        }
    }

    private static NotificationPreferenceDto MapToDto(NotificationPreference preference)
    {
        return new NotificationPreferenceDto(
            preference.Id,
            preference.UserId,
            preference.EmailAppointments,
            preference.EmailReminders,
            preference.ReminderDaysBefore
        );
    }

    private string BuildAppointmentReminderHtml(Appointment appointment, User user)
    {
        var timeString = appointment.AppointmentTime.HasValue
            ? appointment.AppointmentTime.Value.ToString("h:mm tt")
            : "All day";

        var petInfo = appointment.Pet != null
            ? $"<strong>Pet:</strong> {System.Net.WebUtility.HtmlEncode(appointment.Pet.Name)}"
            : "";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #D97706 0%, #F59E0B 100%); padding: 20px; border-radius: 8px 8px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 24px;"">Appointment Reminder</h1>
        <p style=""color: rgba(255,255,255,0.9); margin: 5px 0 0 0;"">Pawsitive Haven</p>
    </div>

    <div style=""background: #fff; border: 1px solid #e5e5e5; border-top: none; padding: 20px; border-radius: 0 0 8px 8px;"">
        <p>Hi {System.Net.WebUtility.HtmlEncode(user.Username)},</p>

        <p>This is a reminder about your upcoming appointment:</p>

        <div style=""background: #FFF7ED; padding: 15px; border-radius: 8px; border-left: 4px solid #D97706; margin: 20px 0;"">
            <h2 style=""color: #D97706; margin: 0 0 10px 0; font-size: 18px;"">{System.Net.WebUtility.HtmlEncode(appointment.Title)}</h2>
            <p style=""margin: 5px 0;""><strong>Date:</strong> {appointment.AppointmentDate:MMMM dd, yyyy}</p>
            <p style=""margin: 5px 0;""><strong>Time:</strong> {timeString}</p>
            {(string.IsNullOrEmpty(petInfo) ? "" : $"<p style=\"margin: 5px 0;\">{petInfo}</p>")}
            {(string.IsNullOrEmpty(appointment.Description) ? "" : $"<p style=\"margin: 10px 0 0 0; color: #666;\">{System.Net.WebUtility.HtmlEncode(appointment.Description)}</p>")}
        </div>

        <p style=""color: #666;"">If you need to reschedule, please update your appointment in the Pawsitive Haven app.</p>

        <div style=""margin-top: 24px; padding-top: 20px; border-top: 1px solid #eee; text-align: center; color: #999; font-size: 12px;"">
            <p>This reminder was sent by Pawsitive Haven.</p>
            <p>You can manage your notification preferences in the app settings.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string BuildAppointmentReminderPlainText(Appointment appointment, User user)
    {
        var timeString = appointment.AppointmentTime.HasValue
            ? appointment.AppointmentTime.Value.ToString("h:mm tt")
            : "All day";

        var petInfo = appointment.Pet != null ? $"\nPet: {appointment.Pet.Name}" : "";

        return $@"APPOINTMENT REMINDER - Pawsitive Haven
======================================

Hi {user.Username},

This is a reminder about your upcoming appointment:

{appointment.Title}
-------------------
Date: {appointment.AppointmentDate:MMMM dd, yyyy}
Time: {timeString}{petInfo}
{(string.IsNullOrEmpty(appointment.Description) ? "" : $"\n{appointment.Description}")}

If you need to reschedule, please update your appointment in the Pawsitive Haven app.

---
This reminder was sent by Pawsitive Haven.
You can manage your notification preferences in the app settings.";
    }

    private string BuildTasksDigestHtml(List<Appointment> tasks, User user)
    {
        var taskRows = string.Join("", tasks.Select(t =>
        {
            var timeString = t.AppointmentTime.HasValue ? t.AppointmentTime.Value.ToString("h:mm tt") : "All day";
            var petName = t.Pet?.Name ?? "-";
            return $@"
                <tr>
                    <td style=""padding: 12px; border-bottom: 1px solid #eee;"">{System.Net.WebUtility.HtmlEncode(t.Title)}</td>
                    <td style=""padding: 12px; border-bottom: 1px solid #eee;"">{t.AppointmentDate:MMM dd}</td>
                    <td style=""padding: 12px; border-bottom: 1px solid #eee;"">{timeString}</td>
                    <td style=""padding: 12px; border-bottom: 1px solid #eee;"">{System.Net.WebUtility.HtmlEncode(petName)}</td>
                </tr>";
        }));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #D97706 0%, #F59E0B 100%); padding: 20px; border-radius: 8px 8px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 24px;"">Upcoming Tasks Digest</h1>
        <p style=""color: rgba(255,255,255,0.9); margin: 5px 0 0 0;"">Pawsitive Haven</p>
    </div>

    <div style=""background: #fff; border: 1px solid #e5e5e5; border-top: none; padding: 20px; border-radius: 0 0 8px 8px;"">
        <p>Hi {System.Net.WebUtility.HtmlEncode(user.Username)},</p>

        <p>Here are your upcoming appointments and tasks:</p>

        <table style=""width: 100%; border-collapse: collapse; margin: 20px 0;"">
            <thead>
                <tr style=""background: #f5f5f5;"">
                    <th style=""padding: 12px; text-align: left; border-bottom: 2px solid #D97706;"">Task</th>
                    <th style=""padding: 12px; text-align: left; border-bottom: 2px solid #D97706;"">Date</th>
                    <th style=""padding: 12px; text-align: left; border-bottom: 2px solid #D97706;"">Time</th>
                    <th style=""padding: 12px; text-align: left; border-bottom: 2px solid #D97706;"">Pet</th>
                </tr>
            </thead>
            <tbody>
                {taskRows}
            </tbody>
        </table>

        <p style=""color: #666;"">Log in to Pawsitive Haven to view details or make changes.</p>

        <div style=""margin-top: 24px; padding-top: 20px; border-top: 1px solid #eee; text-align: center; color: #999; font-size: 12px;"">
            <p>This digest was sent by Pawsitive Haven.</p>
            <p>You can manage your notification preferences in the app settings.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string BuildTasksDigestPlainText(List<Appointment> tasks, User user)
    {
        var taskLines = string.Join("\n", tasks.Select(t =>
        {
            var timeString = t.AppointmentTime.HasValue ? t.AppointmentTime.Value.ToString("h:mm tt") : "All day";
            var petName = t.Pet?.Name ?? "-";
            return $"  - {t.Title} | {t.AppointmentDate:MMM dd} at {timeString} | Pet: {petName}";
        }));

        return $@"UPCOMING TASKS DIGEST - Pawsitive Haven
========================================

Hi {user.Username},

Here are your upcoming appointments and tasks:

{taskLines}

Log in to Pawsitive Haven to view details or make changes.

---
This digest was sent by Pawsitive Haven.
You can manage your notification preferences in the app settings.";
    }
}
