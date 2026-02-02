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

    public async Task<IEnumerable<Pet>> GetByFosterIdAsync(int fosterId)
    {
        return await _dbSet
            .Include(p => p.Foster)
            .Where(p => p.FosterId == fosterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pet>> GetUnassignedPetsAsync()
    {
        return await _dbSet
            .Where(p => p.FosterId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pet>> GetAllWithFosterAsync()
    {
        return await _dbSet
            .Include(p => p.Foster)
            .ToListAsync();
    }

    public async Task<Pet?> GetByIdWithFosterAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Foster)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
