using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(Guid id);
        Task<Client?> GetByEmailAsync(string email);
        Task<List<Client>> GetAllAsync();
        Task<Client> AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(Guid id);
    }
}