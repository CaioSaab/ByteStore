using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IClientService
    {
        Task<AccountResponseDTO> CreateClient(CreateAccountDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
        Task<AccountResponseDTO?> GetById(Guid id);
        Task<List<AccountResponseDTO>> GetAllClients();
        Task<AccountResponseDTO?> UpdateClient(Guid id, UpdateAccountDTO dto);
        Task<bool> DeleteClient(Guid id);
    }
}