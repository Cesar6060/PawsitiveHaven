using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IEscalationRepository : IRepository<Escalation>
{
    Task<List<Escalation>> GetByStatusAsync(string status, int page = 1, int pageSize = 20);
    Task<int> GetCountByStatusAsync(string status);
    Task<List<Escalation>> GetByUserIdAsync(int userId);
    Task<Escalation?> GetByIdWithConversationAsync(int id);
}

public class EscalationRepository : Repository<Escalation>, IEscalationRepository
{
    public EscalationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Escalation>> GetByStatusAsync(string status, int page = 1, int pageSize = 20)
    {
        return await _dbSet
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountByStatusAsync(string status)
    {
        return await _dbSet.CountAsync(e => e.Status == status);
    }

    public async Task<List<Escalation>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Escalation?> GetByIdWithConversationAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Conversation)
                .ThenInclude(c => c.Messages)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
