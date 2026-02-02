using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
{
    Task<NotificationPreference?> GetByUserIdAsync(int userId);
}
