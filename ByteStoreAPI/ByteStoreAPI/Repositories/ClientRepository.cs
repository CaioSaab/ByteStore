using ByteStoreAPI.Data;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ByteStoreDbContext _dbContext;

        public ClientRepository(ByteStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Client> AddAsync(Client client)
        {
            await _dbContext.Clients.AddAsync(client);
            await _dbContext.SaveChangesAsync();
            return client;
        }

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Clients.FindAsync(id);
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _dbContext.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<List<Client>> GetAllAsync()
        {
            return await _dbContext.Clients.ToListAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var client = await GetByIdAsync(id);
            if (client != null)
            {
                _dbContext.Clients.Remove(client);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}