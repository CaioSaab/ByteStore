using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface ICouponRepository
    {
        Task<List<Cupom>> GetByVendorIdAsync(Guid vendorId);
        Task<Cupom?> GetByCodeAsync(Guid vendorId, string code);
        Task<Cupom?> GetActiveByCodeAsync(string code);
        Task<List<Cupom>> GetAllActiveAsync();
        Task<Cupom> UpsertAsync(Guid vendorId, string code, decimal discountPercentage, bool isActive);
        Task<bool> ToggleActiveAsync(Guid vendorId, string code, bool isActive);
        Task<bool> DeleteAsync(Guid vendorId, string code);
    }
}