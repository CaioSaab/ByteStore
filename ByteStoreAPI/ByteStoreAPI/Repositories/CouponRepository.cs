using ByteStoreAPI.Data;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ByteStoreDbContext _db;

        public CouponRepository(ByteStoreDbContext db)
        {
            _db = db;
        }

        public async Task<List<Cupom>> GetByVendorIdAsync(Guid vendorId)
        {
            return await _db.Cupons.Where(c => c.VendorId == vendorId).OrderBy(c => c.Code).ToListAsync();
        }

        public async Task<Cupom?> GetByCodeAsync(Guid vendorId, string code)
        {
            var normalized = code.ToUpper();
            return await _db.Cupons.FirstOrDefaultAsync(c => c.VendorId == vendorId && c.Code == normalized);
        }

        public async Task<Cupom?> GetActiveByCodeAsync(string code)
        {
            var normalized = code.ToUpper();
            return await _db.Cupons.FirstOrDefaultAsync(c => c.IsActive && c.Code == normalized);
        }

        public async Task<List<Cupom>> GetAllActiveAsync()
        {
            return await _db.Cupons.Where(c => c.IsActive).OrderBy(c => c.Code).ToListAsync();
        }

        public async Task<Cupom> UpsertAsync(Guid vendorId, string code, decimal discountPercentage, bool isActive)
        {
            var normalized = code.ToUpper();
            var existing = await _db.Cupons.FirstOrDefaultAsync(c => c.VendorId == vendorId && c.Code == normalized);
            if (existing == null)
            {
                var coupon = new Cupom
                {
                    Id = Guid.NewGuid(),
                    VendorId = vendorId,
                    Code = normalized,
                    DiscountPercentage = discountPercentage,
                    IsActive = isActive
                };
                await _db.Cupons.AddAsync(coupon);
                await _db.SaveChangesAsync();
                return coupon;
            }
            existing.DiscountPercentage = discountPercentage;
            existing.IsActive = isActive;
            _db.Cupons.Update(existing);
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> ToggleActiveAsync(Guid vendorId, string code, bool isActive)
        {
            var normalized = code.ToUpper();
            var existing = await _db.Cupons.FirstOrDefaultAsync(c => c.VendorId == vendorId && c.Code == normalized);
            if (existing == null) return false;
            existing.IsActive = isActive;
            _db.Cupons.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid vendorId, string code)
        {
            var normalized = code.ToUpper();
            var existing = await _db.Cupons.FirstOrDefaultAsync(c => c.VendorId == vendorId && c.Code == normalized);
            if (existing == null) return false;
            _db.Cupons.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}