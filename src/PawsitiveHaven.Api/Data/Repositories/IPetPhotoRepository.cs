using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IPetPhotoRepository : IRepository<PetPhoto>
{
    Task<IEnumerable<PetPhoto>> GetByPetIdAsync(int petId);
    Task<PetPhoto?> GetPrimaryPhotoAsync(int petId);
    Task ClearPrimaryFlagAsync(int petId);
}
