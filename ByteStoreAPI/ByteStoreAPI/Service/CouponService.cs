using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;

namespace ByteStoreAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _repo;

        public CouponService(ICouponRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CouponDTO>> GetVendorCouponsAsync(Guid vendorId)
        {
            var list = await _repo.GetByVendorIdAsync(vendorId);
            return list.Select(Map).ToList();
        }

        public async Task<CouponDTO> UpsertAsync(Guid vendorId, UpsertCouponDTO dto)
        {
            var coupon = await _repo.UpsertAsync(vendorId, dto.Code, dto.DiscountPercentage, dto.IsActive);
            return Map(coupon);
        }

        public async Task<bool> ToggleActiveAsync(Guid vendorId, string code, bool active)
        {
            return await _repo.ToggleActiveAsync(vendorId, code, active);
        }

        public async Task<bool> DeleteAsync(Guid vendorId, string code)
        {
            return await _repo.DeleteAsync(vendorId, code);
        }

        public async Task<CouponDTO?> ValidateAsync(string code)
        {
            var coupon = await _repo.GetActiveByCodeAsync(code);
            return coupon == null ? null : Map(coupon);
        }

        public async Task<List<CouponDTO>> GetAvailableAsync()
        {
            var list = await _repo.GetAllActiveAsync();
            return list.Select(Map).ToList();
        }

        private static CouponDTO Map(Cupom c)
        {
            return new CouponDTO
            {
                Code = c.Code,
                DiscountPercentage = c.DiscountPercentage,
                IsActive = c.IsActive
            };
        }
    }
}