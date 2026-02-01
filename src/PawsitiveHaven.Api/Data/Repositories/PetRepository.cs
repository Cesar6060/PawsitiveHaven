using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public class PetRepository : Repository<Pet>, IPetRepository
{
    public PetRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Pet>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(p => p.UserId == userId).ToListAsync();
    }
}
