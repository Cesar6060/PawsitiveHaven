using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public class PetPhotoRepository : Repository<PetPhoto>, IPetPhotoRepository
{
    public PetPhotoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PetPhoto>> GetByPetIdAsync(int petId)
    {
        return await _dbSet
            .Where(p => p.PetId == petId)
            .OrderByDescending(p => p.IsPrimary)
            .ThenByDescending(p => p.UploadedAt)
            .ToListAsync();
    }

    public async Task<PetPhoto?> GetPrimaryPhotoAsync(int petId)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.PetId == petId && p.IsPrimary);
    }

    public async Task ClearPrimaryFlagAsync(int petId)
    {
        var photos = await _dbSet.Where(p => p.PetId == petId && p.IsPrimary).ToListAsync();
        foreach (var photo in photos)
        {
            photo.IsPrimary = false;
        }
        await _context.SaveChangesAsync();
    }
}
