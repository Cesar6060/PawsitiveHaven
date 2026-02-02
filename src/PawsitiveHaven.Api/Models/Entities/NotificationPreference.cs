namespace PawsitiveHaven.Api.Models.Entities;

public class NotificationPreference
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool EmailAppointments { get; set; } = true;
    public bool EmailReminders { get; set; } = true;
    public int ReminderDaysBefore { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
}
