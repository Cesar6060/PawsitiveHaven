using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IPetRepository : IRepository<Pet>
{
    Task<IEnumerable<Pet>> GetByUserIdAsync(int userId);
}
