using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data.Repositories;

public interface IFaqRepository : IRepository<Faq>
{
    Task<IEnumerable<Faq>> GetActiveFaqsAsync();
    Task<IEnumerable<Faq>> GetAllOrderedAsync();
}
