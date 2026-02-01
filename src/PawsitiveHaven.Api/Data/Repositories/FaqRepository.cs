using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public class FaqRepository : Repository<Faq>, IFaqRepository
{
    public FaqRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Faq>> GetActiveFaqsAsync()
    {
        return await _dbSet
            .Where(f => f.IsActive)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<Faq>> GetAllOrderedAsync()
    {
        return await _dbSet
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();
    }
}
