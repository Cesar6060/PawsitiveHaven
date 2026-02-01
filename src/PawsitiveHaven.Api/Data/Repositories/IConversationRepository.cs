using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId);
    Task<Conversation?> GetByIdWithMessagesAsync(int id);
}
