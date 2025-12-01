using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IVendorRepository
    {
        Task<Vendor> AddAsync(Vendor vendor);
        Task<Vendor?> GetByIdAsync(Guid id);
        Task<Vendor?> GetByEmailAsync(string email);
        Task<List<Vendor>> GetAllAsync();
        Task UpdateAsync(Vendor vendor);
        Task DeleteAsync(Guid id);
    }
}