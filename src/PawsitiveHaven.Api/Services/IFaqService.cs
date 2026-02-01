using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IFaqService
{
    Task<IEnumerable<FaqDto>> GetActiveFaqsAsync();
    Task<IEnumerable<FaqDto>> GetAllFaqsAsync();
    Task<FaqDto?> GetFaqByIdAsync(int id);
    Task<FaqDto?> CreateFaqAsync(CreateFaqRequest request);
    Task<FaqDto?> UpdateFaqAsync(int id, UpdateFaqRequest request);
    Task<bool> DeleteFaqAsync(int id);
}
