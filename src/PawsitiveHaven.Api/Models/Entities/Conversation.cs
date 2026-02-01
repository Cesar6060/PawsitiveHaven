namespace PawsitiveHaven.Api.Models.Entities;

public class Conversation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<ConversationMessage> Messages { get; set; } = new List<ConversationMessage>();
}
