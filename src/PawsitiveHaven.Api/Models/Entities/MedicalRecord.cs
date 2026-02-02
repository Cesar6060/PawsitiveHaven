namespace PawsitiveHaven.Api.Models.Entities;

public class MedicalRecord
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string RecordType { get; set; } = string.Empty; // Vaccination, VetVisit, Medication, Surgery, Other
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly RecordDate { get; set; }
    public DateOnly? NextDueDate { get; set; }
    public string? Veterinarian { get; set; }
    public string? ClinicName { get; set; }
    public decimal? Cost { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    // Navigation properties
    public Pet Pet { get; set; } = null!;
    public User? Creator { get; set; }
}
