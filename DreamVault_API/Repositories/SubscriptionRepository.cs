using DreamVault_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository

    {

        private readonly PostgresContext _db;

        public SubscriptionRepository(PostgresContext db)
        {
            _db = db;
        }
        public async Task<Subscriber> CreateAsync(Subscriber subscriber)
        {
            await _db.Subscribers.AddAsync(subscriber);
            await _db.SaveChangesAsync();
            return subscriber;
        }

        public async Task<Subscriber> GetByCustomerId(string id)
        {
            return await _db.Subscribers.SingleOrDefaultAsync(s => s.CustomerId == id);
        }

        public async Task<Subscriber> GetByIdAsync(string id)
        {
            return await _db.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subscriber> UpdateAsync(Subscriber subscriber)
        {
            _db.Subscribers.UpdateRange(subscriber);
            await _db.SaveChangesAsync();
            return subscriber;
        }
    }
}
