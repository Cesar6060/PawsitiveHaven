namespace PawsitiveHaven.Api.Models.Entities;

public class Pet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public string? Sex { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public int? FosterId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public string? AssignmentNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public User? Foster { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<PetPhoto> Photos { get; set; } = new List<PetPhoto>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
