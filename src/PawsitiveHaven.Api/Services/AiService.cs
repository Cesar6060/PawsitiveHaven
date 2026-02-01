using OpenAI;
using OpenAI.Chat;
using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class AiService : IAiService
{
    private readonly IConversationRepository _conversationRepo;
    private readonly IFaqRepository _faqRepo;
    private readonly ChatClient _chatClient;
    private readonly ILogger<AiService> _logger;

    private const string SystemPrompt = @"You are a helpful assistant for Pawsitive Haven Pet Rescue.
You help users with pet care questions, adoption information, and general pet-related inquiries.
Be friendly, warm, and supportive. If you don't know something specific about the shelter,
suggest the user contact staff directly.

Key information about Pawsitive Haven:
- We are a pet rescue organization helping animals find forever homes
- We provide adoption services, pet care resources, and support for pet owners
- Our mission is to connect loving pets with caring families";

    public AiService(
        IConversationRepository conversationRepo,
        IFaqRepository faqRepo,
        IConfiguration configuration,
        ILogger<AiService> logger)
    {
        _conversationRepo = conversationRepo;
        _faqRepo = faqRepo;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OpenAI API key not configured");

        var client = new OpenAIClient(apiKey);
        _chatClient = client.GetChatClient("gpt-4o-mini");
    }

    public async Task<ChatResponse> ChatAsync(int userId, ChatRequest request)
    {
        try
        {
            Conversation conversation;

            if (request.ConversationId.HasValue)
            {
                conversation = await _conversationRepo.GetByIdWithMessagesAsync(request.ConversationId.Value)
                    ?? throw new InvalidOperationException("Conversation not found");

                if (conversation.UserId != userId)
                    throw new UnauthorizedAccessException("Access denied to conversation");
            }
            else
            {
                conversation = new Conversation
                {
                    UserId = userId,
                    Title = request.Message.Length > 50
                        ? request.Message[..47] + "..."
                        : request.Message
                };
                await _conversationRepo.AddAsync(conversation);
            }

            // Add user message
            var userMessage = new ConversationMessage
            {
                ConversationId = conversation.Id,
                Role = "user",
                Content = request.Message
            };
            conversation.Messages.Add(userMessage);
            await _conversationRepo.UpdateAsync(conversation);

            // Build messages for OpenAI
            var messages = await BuildChatMessagesAsync(conversation);

            // Call OpenAI
            var completion = await _chatClient.CompleteChatAsync(messages);
            var assistantContent = completion.Value.Content[0].Text;

            // Save assistant response
            var assistantMessage = new ConversationMessage
            {
                ConversationId = conversation.Id,
                Role = "assistant",
                Content = assistantContent
            };
            conversation.Messages.Add(assistantMessage);
            await _conversationRepo.UpdateAsync(conversation);

            return new ChatResponse(
                true,
                assistantContent,
                conversation.Id,
                null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in chat completion");
            return new ChatResponse(false, null, null, "Failed to get response. Please try again.");
        }
    }

    public async Task<string> GeneratePetBioAsync(PetBioRequest request)
    {
        var prompt = $@"Write a short, heartwarming bio (2-3 sentences) for a pet available for adoption:
- Name: {request.Name}
- Species: {request.Species}
- Breed: {request.Breed ?? "Mixed"}
- Age: {request.Age?.ToString() ?? "Unknown"} years
- Sex: {request.Sex ?? "Unknown"}
{(string.IsNullOrEmpty(request.Personality) ? "" : $"- Personality traits: {request.Personality}")}

Make it engaging and help potential adopters connect with this pet. Focus on their personality and what makes them special.";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a creative writer helping animal shelters write compelling pet bios."),
            new UserChatMessage(prompt)
        };

        var completion = await _chatClient.CompleteChatAsync(messages);
        return completion.Value.Content[0].Text;
    }

    public async Task<List<ConversationDto>> GetConversationsAsync(int userId)
    {
        var conversations = await _conversationRepo.GetByUserIdAsync(userId);
        return conversations.Select(c => new ConversationDto(
            c.Id,
            c.Title ?? "New Chat",
            c.CreatedAt,
            c.UpdatedAt
        )).ToList();
    }

    public async Task<ConversationDto?> GetConversationAsync(int userId, int conversationId)
    {
        var conversation = await _conversationRepo.GetByIdWithMessagesAsync(conversationId);
        if (conversation == null || conversation.UserId != userId)
            return null;

        return new ConversationDto(
            conversation.Id,
            conversation.Title ?? "New Chat",
            conversation.CreatedAt,
            conversation.UpdatedAt,
            conversation.Messages.Select(m => new MessageDto(m.Role, m.Content, m.CreatedAt)).ToList()
        );
    }

    public async Task<bool> DeleteConversationAsync(int userId, int conversationId)
    {
        var conversation = await _conversationRepo.GetByIdAsync(conversationId);
        if (conversation == null || conversation.UserId != userId)
            return false;

        await _conversationRepo.DeleteAsync(conversation);
        return true;
    }

    private async Task<List<ChatMessage>> BuildChatMessagesAsync(Conversation conversation)
    {
        var messages = new List<ChatMessage>();

        // Add system prompt with FAQ context
        var systemPrompt = await BuildSystemPromptWithFaqsAsync();
        messages.Add(new SystemChatMessage(systemPrompt));

        // Add conversation history (limit to last 20 messages for context window)
        var recentMessages = conversation.Messages
            .OrderBy(m => m.CreatedAt)
            .TakeLast(20);

        foreach (var msg in recentMessages)
        {
            if (msg.Role == "user")
                messages.Add(new UserChatMessage(msg.Content));
            else if (msg.Role == "assistant")
                messages.Add(new AssistantChatMessage(msg.Content));
        }

        return messages;
    }

    private async Task<string> BuildSystemPromptWithFaqsAsync()
    {
        var faqs = await _faqRepo.GetActiveFaqsAsync();
        var topFaqs = faqs.Take(10);

        if (!topFaqs.Any())
            return SystemPrompt;

        var faqContext = string.Join("\n\n", topFaqs.Select(f => $"Q: {f.Question}\nA: {f.Answer}"));

        return $@"{SystemPrompt}

Here are some frequently asked questions you can reference:

{faqContext}";
    }
}
