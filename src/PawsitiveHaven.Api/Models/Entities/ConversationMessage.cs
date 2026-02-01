namespace PawsitiveHaven.Api.Models.Entities;

public class ConversationMessage
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string Role { get; set; } = string.Empty; // "user", "assistant", "system"
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Conversation Conversation { get; set; } = null!;
}
