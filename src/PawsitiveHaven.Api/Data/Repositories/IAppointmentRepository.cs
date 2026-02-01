using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Appointment>> GetByUserIdWithPetAsync(int userId);
    Task<IEnumerable<Appointment>> GetUpcomingByUserIdAsync(int userId, int days = 7);
}
