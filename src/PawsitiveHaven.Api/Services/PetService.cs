using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PetService> _logger;

    public PetService(IPetRepository petRepository, IUserRepository userRepository, ILogger<PetService> logger)
    {
        _petRepository = petRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PetDto>> GetUserPetsAsync(int userId)
    {
        var pets = await _petRepository.GetByUserIdAsync(userId);
        return pets.Select(MapToDto);
    }

    public async Task<PetDto?> GetPetByIdAsync(int id, int userId)
    {
        var pet = await _petRepository.GetByIdAsync(id);
        if (pet == null || pet.UserId != userId)
            return null;

        return MapToDto(pet);
    }

    public async Task<PetDto?> CreatePetAsync(int userId, CreatePetRequest request)
    {
        try
        {
            var pet = new Pet
            {
                UserId = userId,
                Name = request.Name,
                Species = request.Species,
                Breed = request.Breed,
                Age = request.Age,
                Sex = request.Sex,
                Bio = request.Bio,
                ImageUrl = request.ImageUrl
            };

            await _petRepository.AddAsync(pet);
            _logger.LogInformation("Pet created: {PetName} for user {UserId}", pet.Name, userId);

            return MapToDto(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pet for user {UserId}", userId);
            return null;
        }
    }

    public async Task<PetDto?> UpdatePetAsync(int id, int userId, UpdatePetRequest request)
    {
        try
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null || pet.UserId != userId)
                return null;

            if (request.Name != null) pet.Name = request.Name;
            if (request.Species != null) pet.Species = request.Species;
            if (request.Breed != null) pet.Breed = request.Breed;
            if (request.Age.HasValue) pet.Age = request.Age;
            if (request.Sex != null) pet.Sex = request.Sex;
            if (request.Bio != null) pet.Bio = request.Bio;
            if (request.ImageUrl != null) pet.ImageUrl = request.ImageUrl;

            pet.UpdatedAt = DateTime.UtcNow;

            await _petRepository.UpdateAsync(pet);
            _logger.LogInformation("Pet updated: {PetId} for user {UserId}", id, userId);

            return MapToDto(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pet {PetId} for user {UserId}", id, userId);
            return null;
        }
    }

    public async Task<bool> DeletePetAsync(int id, int userId)
    {
        try
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null || pet.UserId != userId)
                return false;

            await _petRepository.DeleteAsync(pet);
            _logger.LogInformation("Pet deleted: {PetId} for user {UserId}", id, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pet {PetId} for user {UserId}", id, userId);
            return false;
        }
    }

    public async Task<PetDto?> AssignPetToFosterAsync(int petId, AssignPetRequest request)
    {
        try
        {
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                return null;
            }

            var foster = await _userRepository.GetByIdAsync(request.FosterId);
            if (foster == null)
            {
                _logger.LogWarning("Foster not found: {FosterId}", request.FosterId);
                return null;
            }

            pet.FosterId = request.FosterId;
            pet.AssignedAt = DateTime.UtcNow;
            pet.AssignmentNotes = request.Notes;
            pet.UpdatedAt = DateTime.UtcNow;

            await _petRepository.UpdateAsync(pet);
            _logger.LogInformation("Pet {PetId} assigned to foster {FosterId}", petId, request.FosterId);

            return new PetDto(
                pet.Id,
                pet.UserId,
                pet.Name,
                pet.Species,
                pet.Breed,
                pet.Age,
                pet.Sex,
                pet.Bio,
                pet.ImageUrl,
                pet.FosterId,
                foster.Username,
                pet.AssignedAt,
                pet.AssignmentNotes
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning pet {PetId} to foster", petId);
            return null;
        }
    }

    public async Task<PetDto?> UnassignPetAsync(int petId)
    {
        try
        {
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                return null;
            }

            pet.FosterId = null;
            pet.AssignedAt = null;
            pet.AssignmentNotes = null;
            pet.UpdatedAt = DateTime.UtcNow;

            await _petRepository.UpdateAsync(pet);
            _logger.LogInformation("Pet {PetId} unassigned from foster", petId);

            return MapToDto(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning pet {PetId}", petId);
            return null;
        }
    }

    public async Task<IEnumerable<PetDto>> GetUnassignedPetsAsync()
    {
        var pets = await _petRepository.GetUnassignedPetsAsync();
        return pets.Select(MapToDto);
    }

    public async Task<IEnumerable<PetDto>> GetPetsByFosterIdAsync(int fosterId)
    {
        var pets = await _petRepository.GetByFosterIdAsync(fosterId);
        return pets.Select(p => new PetDto(
            p.Id,
            p.UserId,
            p.Name,
            p.Species,
            p.Breed,
            p.Age,
            p.Sex,
            p.Bio,
            p.ImageUrl,
            p.FosterId,
            p.Foster?.Username,
            p.AssignedAt,
            p.AssignmentNotes
        ));
    }

    public async Task<IEnumerable<PetDto>> GetAllPetsWithFosterAsync()
    {
        var pets = await _petRepository.GetAllWithFosterAsync();
        return pets.Select(p => new PetDto(
            p.Id,
            p.UserId,
            p.Name,
            p.Species,
            p.Breed,
            p.Age,
            p.Sex,
            p.Bio,
            p.ImageUrl,
            p.FosterId,
            p.Foster?.Username,
            p.AssignedAt,
            p.AssignmentNotes
        ));
    }

    public async Task<PetDto?> GetPetByIdWithFosterAsync(int id)
    {
        var pet = await _petRepository.GetByIdWithFosterAsync(id);
        if (pet == null)
            return null;

        return new PetDto(
            pet.Id,
            pet.UserId,
            pet.Name,
            pet.Species,
            pet.Breed,
            pet.Age,
            pet.Sex,
            pet.Bio,
            pet.ImageUrl,
            pet.FosterId,
            pet.Foster?.Username,
            pet.AssignedAt,
            pet.AssignmentNotes
        );
    }

    private static PetDto MapToDto(Pet pet)
    {
        return new PetDto(
            pet.Id,
            pet.UserId,
            pet.Name,
            pet.Species,
            pet.Breed,
            pet.Age,
            pet.Sex,
            pet.Bio,
            pet.ImageUrl,
            pet.FosterId,
            pet.Foster?.Username,
            pet.AssignedAt,
            pet.AssignmentNotes
        );
    }
}
