namespace PawsitiveHaven.Api.Models.Entities;

public class PetPhoto
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int? UploadedBy { get; set; }

    // Navigation properties
    public Pet Pet { get; set; } = null!;
    public User? Uploader { get; set; }
}
