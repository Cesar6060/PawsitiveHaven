using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public class NotificationPreferenceRepository : Repository<NotificationPreference>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<NotificationPreference?> GetByUserIdAsync(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(np => np.UserId == userId);
    }
}
