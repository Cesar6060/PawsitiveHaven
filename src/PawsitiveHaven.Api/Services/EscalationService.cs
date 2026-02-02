using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public interface IEscalationService
{
    Task<EscalationResponse> CreateEscalationAsync(int userId, CreateEscalationRequest request);
    Task<EscalationResponse?> GetEscalationAsync(int escalationId, int? userId = null);
    Task<EscalationListResponse> GetPendingEscalationsAsync(int page = 1, int pageSize = 20);
    Task<EscalationListResponse> GetEscalationsByStatusAsync(string status, int page = 1, int pageSize = 20);
    Task<EscalationResponse?> UpdateEscalationAsync(int escalationId, UpdateEscalationRequest request);
    Task<List<EscalationResponse>> GetUserEscalationsAsync(int userId);
}

public class EscalationService : IEscalationService
{
    private readonly IEscalationRepository _escalationRepo;
    private readonly IConversationRepository _conversationRepo;
    private readonly IEmailService _emailService;
    private readonly ILogger<EscalationService> _logger;

    public EscalationService(
        IEscalationRepository escalationRepo,
        IConversationRepository conversationRepo,
        IEmailService emailService,
        ILogger<EscalationService> logger)
    {
        _escalationRepo = escalationRepo;
        _conversationRepo = conversationRepo;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<EscalationResponse> CreateEscalationAsync(int userId, CreateEscalationRequest request)
    {
        // Validate conversation belongs to user
        var conversation = await _conversationRepo.GetByIdWithMessagesAsync(request.ConversationId);
        if (conversation == null)
        {
            throw new InvalidOperationException("Conversation not found");
        }

        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("Access denied to conversation");
        }

        var escalation = new Escalation
        {
            ConversationId = request.ConversationId,
            UserId = userId,
            MessageId = request.MessageId,
            UserEmail = request.UserEmail,
            UserName = request.UserName,
            UserQuestion = request.UserQuestion,
            AdditionalContext = request.AdditionalContext,
            Status = "Pending"
        };

        await _escalationRepo.AddAsync(escalation);
        _logger.LogInformation("Created escalation {EscalationId} for user {UserId}", escalation.Id, userId);

        // Send email notification
        var conversationMessages = conversation.Messages.ToList();
        var emailSent = await _emailService.SendEscalationEmailAsync(escalation, conversationMessages);

        if (emailSent)
        {
            escalation.EmailSentAt = DateTime.UtcNow;
            await _escalationRepo.UpdateAsync(escalation);
        }

        return MapToResponse(escalation);
    }

    public async Task<EscalationResponse?> GetEscalationAsync(int escalationId, int? userId = null)
    {
        var escalation = await _escalationRepo.GetByIdAsync(escalationId);
        if (escalation == null)
            return null;

        // If userId is provided, validate access
        if (userId.HasValue && escalation.UserId != userId.Value)
            return null;

        return MapToResponse(escalation);
    }

    public async Task<EscalationListResponse> GetPendingEscalationsAsync(int page = 1, int pageSize = 20)
    {
        return await GetEscalationsByStatusAsync("Pending", page, pageSize);
    }

    public async Task<EscalationListResponse> GetEscalationsByStatusAsync(string status, int page = 1, int pageSize = 20)
    {
        var escalations = await _escalationRepo.GetByStatusAsync(status, page, pageSize);
        var totalCount = await _escalationRepo.GetCountByStatusAsync(status);

        return new EscalationListResponse(
            escalations.Select(MapToResponse).ToList(),
            totalCount,
            page,
            pageSize
        );
    }

    public async Task<EscalationResponse?> UpdateEscalationAsync(int escalationId, UpdateEscalationRequest request)
    {
        var escalation = await _escalationRepo.GetByIdAsync(escalationId);
        if (escalation == null)
            return null;

        if (!string.IsNullOrEmpty(request.Status))
        {
            var validStatuses = new[] { "Pending", "InProgress", "Resolved", "Closed" };
            if (!validStatuses.Contains(request.Status))
            {
                throw new ArgumentException($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");
            }

            escalation.Status = request.Status;

            if (request.Status == "Resolved" || request.Status == "Closed")
            {
                escalation.ResolvedAt = DateTime.UtcNow;
            }
        }

        if (request.StaffNotes != null)
        {
            escalation.StaffNotes = request.StaffNotes;
        }

        await _escalationRepo.UpdateAsync(escalation);
        _logger.LogInformation("Updated escalation {EscalationId}: Status={Status}", escalationId, escalation.Status);

        return MapToResponse(escalation);
    }

    public async Task<List<EscalationResponse>> GetUserEscalationsAsync(int userId)
    {
        var escalations = await _escalationRepo.GetByUserIdAsync(userId);
        return escalations.Select(MapToResponse).ToList();
    }

    private static EscalationResponse MapToResponse(Escalation escalation)
    {
        return new EscalationResponse(
            escalation.Id,
            escalation.ConversationId,
            escalation.UserEmail,
            escalation.UserName,
            escalation.UserQuestion,
            escalation.AdditionalContext,
            escalation.Status,
            escalation.CreatedAt,
            escalation.EmailSentAt,
            escalation.ResolvedAt,
            escalation.StaffNotes
        );
    }
}
