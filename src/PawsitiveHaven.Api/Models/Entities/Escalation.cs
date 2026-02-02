namespace PawsitiveHaven.Api.Models.Entities;

public class Escalation
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public int? MessageId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserQuestion { get; set; } = string.Empty;
    public string? AdditionalContext { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EmailSentAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? StaffNotes { get; set; }

    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
    public ConversationMessage? Message { get; set; }
}
