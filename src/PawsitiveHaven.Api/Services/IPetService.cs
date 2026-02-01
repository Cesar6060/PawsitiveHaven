using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IPetService
{
    Task<IEnumerable<PetDto>> GetUserPetsAsync(int userId);
    Task<PetDto?> GetPetByIdAsync(int id, int userId);
    Task<PetDto?> CreatePetAsync(int userId, CreatePetRequest request);
    Task<PetDto?> UpdatePetAsync(int id, int userId, UpdatePetRequest request);
    Task<bool> DeletePetAsync(int id, int userId);
}
