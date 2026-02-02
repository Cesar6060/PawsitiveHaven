using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class NotificationService
{
    private readonly ApiClient _apiClient;

    public NotificationService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<NotificationPreferenceDto?> GetPreferencesAsync()
    {
        return await _apiClient.GetAsync<NotificationPreferenceDto>("api/notifications/preferences");
    }

    public async Task<NotificationPreferenceDto?> UpdatePreferencesAsync(UpdateNotificationPreferencesRequest request)
    {
        return await _apiClient.PutAsync<UpdateNotificationPreferencesRequest, NotificationPreferenceDto>(
            "api/notifications/preferences", request);
    }
}
