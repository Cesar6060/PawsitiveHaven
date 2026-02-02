using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IPetService
{
    Task<IEnumerable<PetDto>> GetUserPetsAsync(int userId);
    Task<PetDto?> GetPetByIdAsync(int id, int userId);
    Task<PetDto?> CreatePetAsync(int userId, CreatePetRequest request);
    Task<PetDto?> UpdatePetAsync(int id, int userId, UpdatePetRequest request);
    Task<bool> DeletePetAsync(int id, int userId);

    // Foster assignment methods
    Task<PetDto?> AssignPetToFosterAsync(int petId, AssignPetRequest request);
    Task<PetDto?> UnassignPetAsync(int petId);
    Task<IEnumerable<PetDto>> GetUnassignedPetsAsync();
    Task<IEnumerable<PetDto>> GetPetsByFosterIdAsync(int fosterId);
    Task<IEnumerable<PetDto>> GetAllPetsWithFosterAsync();
    Task<PetDto?> GetPetByIdWithFosterAsync(int id);
}
