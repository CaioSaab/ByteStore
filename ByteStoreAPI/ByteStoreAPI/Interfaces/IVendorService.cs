using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IVendorService
    {
        Task<Vendor> RegisterAsync(CreateAccountDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
        Task<AccountResponseDTO?> GetById(Guid id);
        Task<List<AccountResponseDTO>> GetAllVendors();
        Task<AccountResponseDTO?> UpdateVendor(Guid id, UpdateAccountDTO dto);
        Task<bool> DeleteVendor(Guid id);
    }
}