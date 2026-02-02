using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class MedicalRecordService
{
    private readonly ApiClient _apiClient;

    public MedicalRecordService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<MedicalRecordDto>> GetPetMedicalRecordsAsync(int petId)
    {
        var records = await _apiClient.GetAsync<List<MedicalRecordDto>>($"api/pets/{petId}/medical-records");
        return records ?? new List<MedicalRecordDto>();
    }

    public async Task<MedicalRecordDto?> GetMedicalRecordAsync(int petId, int id)
    {
        return await _apiClient.GetAsync<MedicalRecordDto>($"api/pets/{petId}/medical-records/{id}");
    }

    public async Task<MedicalRecordDto?> CreateMedicalRecordAsync(int petId, CreateMedicalRecordRequest request)
    {
        return await _apiClient.PostAsync<CreateMedicalRecordRequest, MedicalRecordDto>($"api/pets/{petId}/medical-records", request);
    }

    public async Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int petId, int id, UpdateMedicalRecordRequest request)
    {
        return await _apiClient.PutAsync<UpdateMedicalRecordRequest, MedicalRecordDto>($"api/pets/{petId}/medical-records/{id}", request);
    }

    public async Task<bool> DeleteMedicalRecordAsync(int petId, int id)
    {
        return await _apiClient.DeleteAsync($"api/pets/{petId}/medical-records/{id}");
    }

    public async Task<List<UpcomingMedicalRecordDto>> GetUpcomingMedicalRecordsAsync(int days = 30)
    {
        var records = await _apiClient.GetAsync<List<UpcomingMedicalRecordDto>>($"api/medical-records/upcoming?days={days}");
        return records ?? new List<UpcomingMedicalRecordDto>();
    }
}
