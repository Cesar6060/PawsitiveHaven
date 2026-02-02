using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IPetRepository : IRepository<Pet>
{
    Task<IEnumerable<Pet>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Pet>> GetByFosterIdAsync(int fosterId);
    Task<IEnumerable<Pet>> GetUnassignedPetsAsync();
    Task<IEnumerable<Pet>> GetAllWithFosterAsync();
    Task<Pet?> GetByIdWithFosterAsync(int id);
}
