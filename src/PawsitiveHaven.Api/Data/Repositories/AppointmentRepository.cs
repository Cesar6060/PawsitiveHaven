using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByUserIdWithPetAsync(int userId)
    {
        return await _dbSet
            .Include(a => a.Pet)
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingByUserIdAsync(int userId, int days = 7)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = today.AddDays(days);

        return await _dbSet
            .Include(a => a.Pet)
            .Where(a => a.UserId == userId &&
                        a.AppointmentDate >= today &&
                        a.AppointmentDate <= endDate &&
                        !a.IsCompleted)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToListAsync();
    }
}
