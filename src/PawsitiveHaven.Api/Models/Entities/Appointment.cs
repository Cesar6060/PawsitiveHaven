namespace PawsitiveHaven.Api.Models.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? PetId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly? AppointmentTime { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Pet? Pet { get; set; }
}
