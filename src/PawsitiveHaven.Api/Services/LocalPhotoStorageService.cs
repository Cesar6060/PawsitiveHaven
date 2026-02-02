namespace PawsitiveHaven.Api.Services;

public class LocalPhotoStorageService : IPhotoStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalPhotoStorageService> _logger;

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const string UploadsFolder = "uploads/pets";

    public LocalPhotoStorageService(IWebHostEnvironment environment, ILogger<LocalPhotoStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<(string FilePath, string FileName)> SavePhotoAsync(Stream fileStream, string originalFileName, int petId)
    {
        try
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            var uniqueFileName = $"{petId}_{Guid.NewGuid()}{extension}";

            var uploadsPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", UploadsFolder);

            // Ensure directory exists
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fullPath = Path.Combine(uploadsPath, uniqueFileName);
            var relativePath = $"{UploadsFolder}/{uniqueFileName}";

            await using var fileStreamOutput = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamOutput);

            _logger.LogInformation("Photo saved: {FilePath} for pet {PetId}", relativePath, petId);

            return (relativePath, uniqueFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving photo for pet {PetId}", petId);
            throw;
        }
    }

    public async Task<bool> DeletePhotoAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", filePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("Photo deleted: {FilePath}", filePath);
                return true;
            }

            _logger.LogWarning("Photo not found for deletion: {FilePath}", filePath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting photo: {FilePath}", filePath);
            return false;
        }
    }

    public string GetPhotoUrl(string filePath)
    {
        // Return the relative URL path for serving static files
        return $"/{filePath}";
    }

    public bool IsValidFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    public bool IsValidFileSize(long fileSize)
    {
        return fileSize > 0 && fileSize <= MaxFileSizeBytes;
    }
}
