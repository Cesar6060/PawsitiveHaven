using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class FaqService : IFaqService
{
    private readonly IFaqRepository _faqRepository;
    private readonly ILogger<FaqService> _logger;

    public FaqService(IFaqRepository faqRepository, ILogger<FaqService> logger)
    {
        _faqRepository = faqRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<FaqDto>> GetActiveFaqsAsync()
    {
        var faqs = await _faqRepository.GetActiveFaqsAsync();
        return faqs.Select(MapToDto);
    }

    public async Task<IEnumerable<FaqDto>> GetAllFaqsAsync()
    {
        var faqs = await _faqRepository.GetAllOrderedAsync();
        return faqs.Select(MapToDto);
    }

    public async Task<FaqDto?> GetFaqByIdAsync(int id)
    {
        var faq = await _faqRepository.GetByIdAsync(id);
        return faq != null ? MapToDto(faq) : null;
    }

    public async Task<FaqDto?> CreateFaqAsync(CreateFaqRequest request)
    {
        try
        {
            var faq = new Faq
            {
                Question = request.Question,
                Answer = request.Answer,
                DisplayOrder = request.DisplayOrder
            };

            await _faqRepository.AddAsync(faq);
            _logger.LogInformation("FAQ created: {FaqId}", faq.Id);

            return MapToDto(faq);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating FAQ");
            return null;
        }
    }

    public async Task<FaqDto?> UpdateFaqAsync(int id, UpdateFaqRequest request)
    {
        try
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            if (faq == null)
                return null;

            if (request.Question != null) faq.Question = request.Question;
            if (request.Answer != null) faq.Answer = request.Answer;
            if (request.DisplayOrder.HasValue) faq.DisplayOrder = request.DisplayOrder.Value;
            if (request.IsActive.HasValue) faq.IsActive = request.IsActive.Value;

            faq.UpdatedAt = DateTime.UtcNow;

            await _faqRepository.UpdateAsync(faq);
            _logger.LogInformation("FAQ updated: {FaqId}", id);

            return MapToDto(faq);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating FAQ {FaqId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteFaqAsync(int id)
    {
        try
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            if (faq == null)
                return false;

            await _faqRepository.DeleteAsync(faq);
            _logger.LogInformation("FAQ deleted: {FaqId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting FAQ {FaqId}", id);
            return false;
        }
    }

    private static FaqDto MapToDto(Faq faq)
    {
        return new FaqDto(
            faq.Id,
            faq.Question,
            faq.Answer,
            faq.DisplayOrder,
            faq.IsActive
        );
    }
}
