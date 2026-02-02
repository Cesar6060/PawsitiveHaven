using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IMedicalRecordService
{
    Task<IEnumerable<MedicalRecordDto>> GetPetMedicalRecordsAsync(int petId, int userId);
    Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int id, int userId);
    Task<MedicalRecordDto?> CreateMedicalRecordAsync(int petId, int userId, CreateMedicalRecordRequest request);
    Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int id, int petId, int userId, UpdateMedicalRecordRequest request);
    Task<bool> DeleteMedicalRecordAsync(int id, int petId, int userId);
    Task<IEnumerable<UpcomingMedicalRecordDto>> GetUpcomingDueDatesAsync(int userId, int daysAhead = 30);
}
