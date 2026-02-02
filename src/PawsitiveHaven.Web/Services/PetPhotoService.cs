using System.Net.Http.Headers;
using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class PetPhotoService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<PetPhotoService> _logger;

    public PetPhotoService(ApiClient apiClient, ILogger<PetPhotoService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<List<PetPhotoDto>> GetPhotosAsync(int petId)
    {
        var photos = await _apiClient.GetAsync<List<PetPhotoDto>>($"api/pets/{petId}/photos");
        return photos ?? new List<PetPhotoDto>();
    }

    public async Task<UploadPhotoResponse?> UploadPhotoAsync(int petId, Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(fileStream);

            // Set content type based on file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "file", fileName);

            return await _apiClient.PostMultipartAsync<UploadPhotoResponse>($"api/pets/{petId}/photos", content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for pet {PetId}", petId);
            return new UploadPhotoResponse(false, "Error uploading photo", null);
        }
    }

    public async Task<bool> DeletePhotoAsync(int petId, int photoId)
    {
        return await _apiClient.DeleteAsync($"api/pets/{petId}/photos/{photoId}");
    }

    public async Task<SetPrimaryPhotoResponse?> SetPrimaryPhotoAsync(int petId, int photoId)
    {
        return await _apiClient.PutEmptyAsync<SetPrimaryPhotoResponse>($"api/pets/{petId}/photos/{photoId}/primary");
    }

    public string GetPhotoUrl(PetPhotoDto photo)
    {
        var baseUrl = _apiClient.GetApiBaseUrl().TrimEnd('/');
        return $"{baseUrl}{photo.Url}";
    }
}
