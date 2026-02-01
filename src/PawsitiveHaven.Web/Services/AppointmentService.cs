using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class AppointmentService
{
    private readonly ApiClient _apiClient;

    public AppointmentService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync()
    {
        var appointments = await _apiClient.GetAsync<List<AppointmentDto>>("api/appointments");
        return appointments ?? new List<AppointmentDto>();
    }

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(int days = 7)
    {
        var appointments = await _apiClient.GetAsync<List<AppointmentDto>>($"api/appointments/upcoming?days={days}");
        return appointments ?? new List<AppointmentDto>();
    }

    public async Task<AppointmentDto?> CreateAppointmentAsync(CreateAppointmentRequest request)
    {
        return await _apiClient.PostAsync<CreateAppointmentRequest, AppointmentDto>("api/appointments", request);
    }

    public async Task<AppointmentDto?> UpdateAppointmentAsync(int id, UpdateAppointmentRequest request)
    {
        return await _apiClient.PutAsync<UpdateAppointmentRequest, AppointmentDto>($"api/appointments/{id}", request);
    }

    public async Task<AppointmentDto?> ToggleCompleteAsync(int id)
    {
        return await _apiClient.PatchAsync<AppointmentDto>($"api/appointments/{id}/complete");
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/appointments/{id}");
    }
}
