using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    Task<IEnumerable<MedicalRecord>> GetByPetIdAsync(int petId);
    Task<IEnumerable<MedicalRecord>> GetUpcomingDueDatesAsync(int userId, int daysAhead = 30);
    Task<MedicalRecord?> GetByIdWithDetailsAsync(int id);
}
