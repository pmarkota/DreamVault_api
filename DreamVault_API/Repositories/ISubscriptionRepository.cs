using DreamVault_API.Models;

namespace DreamVault_API.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscriber> CreateAsync(Subscriber subscriber);
        Task<Subscriber> GetByCustomerId(string id);
        Task<Subscriber> UpdateAsync(Subscriber subscriber);
        Task<Subscriber> GetByIdAsync(string id);
    }
}
