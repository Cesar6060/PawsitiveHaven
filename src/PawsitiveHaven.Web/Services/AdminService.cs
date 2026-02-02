using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class AdminService
{
    private readonly ApiClient _apiClient;

    public AdminService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    // Dashboard
    public async Task<DashboardStats?> GetDashboardStatsAsync()
    {
        return await _apiClient.GetAsync<DashboardStats>("api/admin/dashboard");
    }

    // User Management
    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _apiClient.GetAsync<List<UserDto>>("api/admin/users");
        return users ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetUserAsync(int id)
    {
        return await _apiClient.GetAsync<UserDto>($"api/admin/users/{id}");
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserRequest request)
    {
        return await _apiClient.PostAsync<CreateUserRequest, UserDto>("api/admin/users", request);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        return await _apiClient.PutAsync<UpdateUserRequest, UserDto>($"api/admin/users/{id}", request);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/admin/users/{id}");
    }

    // FAQ Management
    public async Task<List<FaqDto>> GetAllFaqsAsync()
    {
        var faqs = await _apiClient.GetAsync<List<FaqDto>>("api/faqs/all");
        return faqs ?? new List<FaqDto>();
    }

    public async Task<FaqDto?> CreateFaqAsync(CreateFaqRequest request)
    {
        return await _apiClient.PostAsync<CreateFaqRequest, FaqDto>("api/faqs", request);
    }

    public async Task<FaqDto?> UpdateFaqAsync(int id, UpdateFaqRequest request)
    {
        return await _apiClient.PutAsync<UpdateFaqRequest, FaqDto>($"api/faqs/{id}", request);
    }

    public async Task<bool> DeleteFaqAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/faqs/{id}");
    }

    // Escalation Management
    public async Task<EscalationListResponse?> GetEscalationsByStatusAsync(string status, int page = 1, int pageSize = 20)
    {
        return await _apiClient.GetAsync<EscalationListResponse>($"api/escalations/status/{status}?page={page}&pageSize={pageSize}");
    }

    public async Task<EscalationListResponse?> GetPendingEscalationsAsync(int page = 1, int pageSize = 20)
    {
        return await _apiClient.GetAsync<EscalationListResponse>($"api/escalations/pending?page={page}&pageSize={pageSize}");
    }

    public async Task<EscalationResponse?> GetEscalationAsync(int id)
    {
        return await _apiClient.GetAsync<EscalationResponse>($"api/escalations/{id}");
    }

    public async Task<EscalationResponse?> UpdateEscalationAsync(int id, UpdateEscalationRequest request)
    {
        return await _apiClient.PatchAsync<UpdateEscalationRequest, EscalationResponse>($"api/escalations/{id}", request);
    }
}
