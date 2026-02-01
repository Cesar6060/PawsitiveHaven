using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class ChatService
{
    private readonly ApiClient _apiClient;

    public ChatService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ChatResponse> SendMessageAsync(string message, int? conversationId = null)
    {
        var request = new ChatRequest(message, conversationId);
        var response = await _apiClient.PostAsync<ChatRequest, ChatResponse>("api/ai/chat", request);
        return response ?? new ChatResponse(false, null, null, "Connection error");
    }

    public async Task<List<ConversationDto>> GetConversationsAsync()
    {
        var conversations = await _apiClient.GetAsync<List<ConversationDto>>("api/ai/conversations");
        return conversations ?? new List<ConversationDto>();
    }

    public async Task<ConversationDto?> GetConversationAsync(int conversationId)
    {
        return await _apiClient.GetAsync<ConversationDto>($"api/ai/conversations/{conversationId}");
    }

    public async Task<bool> DeleteConversationAsync(int conversationId)
    {
        return await _apiClient.DeleteAsync($"api/ai/conversations/{conversationId}");
    }

    public async Task<PetBioResponse> GeneratePetBioAsync(PetBioRequest request)
    {
        var response = await _apiClient.PostAsync<PetBioRequest, PetBioResponse>("api/ai/generate-bio", request);
        return response ?? new PetBioResponse(false, null, "Connection error");
    }
}
