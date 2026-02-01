using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IAiService
{
    Task<ChatResponse> ChatAsync(int userId, ChatRequest request);
    Task<string> GeneratePetBioAsync(PetBioRequest request);
    Task<List<ConversationDto>> GetConversationsAsync(int userId);
    Task<ConversationDto?> GetConversationAsync(int userId, int conversationId);
    Task<bool> DeleteConversationAsync(int userId, int conversationId);
}
