using ByteStoreAPI.Data;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ByteStoreDbContext _dbContext;

        public VendorRepository(ByteStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Vendor> AddAsync(Vendor vendor)
        {
            await _dbContext.Vendors.AddAsync(vendor);
            await _dbContext.SaveChangesAsync();
            return vendor;
        }

        public async Task<Vendor?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Vendors.FindAsync(id);
        }

        public async Task<Vendor?> GetByEmailAsync(string email)
        {
            return await _dbContext.Vendors.FirstOrDefaultAsync(v => v.Email == email);
        }

        public async Task<List<Vendor>> GetAllAsync()
        {
            return await _dbContext.Vendors.ToListAsync();
        }

        public async Task UpdateAsync(Vendor vendor)
        {
            _dbContext.Vendors.Update(vendor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var vendor = await GetByIdAsync(id);
            if (vendor != null)
            {
                _dbContext.Vendors.Remove(vendor);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}