using ByteStoreAPI.DTOs;

namespace ByteStoreAPI.Interfaces
{
    public interface ICouponService
    {
        Task<List<CouponDTO>> GetVendorCouponsAsync(Guid vendorId);
        Task<CouponDTO> UpsertAsync(Guid vendorId, UpsertCouponDTO dto);
        Task<bool> ToggleActiveAsync(Guid vendorId, string code, bool active);
        Task<bool> DeleteAsync(Guid vendorId, string code);
        Task<CouponDTO?> ValidateAsync(string code);
        Task<List<CouponDTO>> GetAvailableAsync();
    }
}