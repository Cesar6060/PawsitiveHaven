using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class FaqService
{
    private readonly ApiClient _apiClient;

    public FaqService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<FaqDto>> GetFaqsAsync()
    {
        var faqs = await _apiClient.GetAsync<List<FaqDto>>("api/faqs");
        return faqs ?? new List<FaqDto>();
    }
}
