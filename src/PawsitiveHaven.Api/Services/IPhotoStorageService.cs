namespace PawsitiveHaven.Api.Services;

public interface IPhotoStorageService
{
    Task<(string FilePath, string FileName)> SavePhotoAsync(Stream fileStream, string originalFileName, int petId);
    Task<bool> DeletePhotoAsync(string filePath);
    string GetPhotoUrl(string filePath);
    bool IsValidFileType(string fileName);
    bool IsValidFileSize(long fileSize);
}
