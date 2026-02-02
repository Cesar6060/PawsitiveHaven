using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
{
    public MedicalRecordRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MedicalRecord>> GetByPetIdAsync(int petId)
    {
        return await _dbSet
            .Include(m => m.Pet)
            .Include(m => m.Creator)
            .Where(m => m.PetId == petId)
            .OrderByDescending(m => m.RecordDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalRecord>> GetUpcomingDueDatesAsync(int userId, int daysAhead = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var futureDate = today.AddDays(daysAhead);

        return await _dbSet
            .Include(m => m.Pet)
            .Where(m => (m.Pet.UserId == userId || m.Pet.FosterId == userId) &&
                        m.NextDueDate != null &&
                        m.NextDueDate >= today &&
                        m.NextDueDate <= futureDate)
            .OrderBy(m => m.NextDueDate)
            .ToListAsync();
    }

    public async Task<MedicalRecord?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Pet)
            .Include(m => m.Creator)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}
