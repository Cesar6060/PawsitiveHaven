using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class PetService
{
    private readonly ApiClient _apiClient;

    public PetService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<PetDto>> GetPetsAsync()
    {
        var pets = await _apiClient.GetAsync<List<PetDto>>("api/pets");
        return pets ?? new List<PetDto>();
    }

    public async Task<PetDto?> GetPetAsync(int id)
    {
        return await _apiClient.GetAsync<PetDto>($"api/pets/{id}");
    }

    public async Task<PetDto?> CreatePetAsync(CreatePetRequest request)
    {
        return await _apiClient.PostAsync<CreatePetRequest, PetDto>("api/pets", request);
    }

    public async Task<PetDto?> UpdatePetAsync(int id, UpdatePetRequest request)
    {
        return await _apiClient.PutAsync<UpdatePetRequest, PetDto>($"api/pets/{id}", request);
    }

    public async Task<bool> DeletePetAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/pets/{id}");
    }
}
