using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;

namespace ByteStoreAPI.Service
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ITokenService _tokenService;

        public VendorService(IVendorRepository vendorRepository, ITokenService tokenService)
        {
            _vendorRepository = vendorRepository;
            _tokenService = tokenService;
        }

        public async Task<Vendor> RegisterAsync(CreateAccountDTO dto)
        {
            var contaExistente = await _vendorRepository.GetByEmailAsync(dto.Email);
            if (contaExistente != null)
            {
                throw new InvalidOperationException("Este e-mail já está em uso.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var novoVendor = new Vendor(dto.Nome, dto.Email, passwordHash);

            return await _vendorRepository.AddAsync(novoVendor);
        }

        public async Task<string> LoginAsync(LoginDTO dto)
        {
            var vendor = await _vendorRepository.GetByEmailAsync(dto.Email);

            if (vendor == null || !BCrypt.Net.BCrypt.Verify(dto.Password, vendor.PasswordHash))
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            // Gerar token
            return _tokenService.GenerateToken(vendor.Id, vendor.Email, vendor.Nome, "Vendor");
        }

        public async Task<AccountResponseDTO?> GetById(Guid id)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);
            if (vendor == null) return null;

            return new AccountResponseDTO { Id = vendor.Id, Nome = vendor.Nome, Email = vendor.Email };
        }

        public async Task<List<AccountResponseDTO>> GetAllVendors()
        {
            var vendors = await _vendorRepository.GetAllAsync();

            return vendors.Select(v => new AccountResponseDTO
            {
                Id = v.Id,
                Nome = v.Nome,
                Email = v.Email
            }).ToList();
        }

        public async Task<AccountResponseDTO?> UpdateVendor(Guid id, UpdateAccountDTO dto)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);
            if (vendor == null)
            {
                return null;
            }

            vendor.Nome = dto.Nome;
            vendor.Email = dto.Email;

            await _vendorRepository.UpdateAsync(vendor);

            return new AccountResponseDTO { Id = vendor.Id, Nome = vendor.Nome, Email = vendor.Email };
        }

        public async Task<bool> DeleteVendor(Guid id)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);
            if (vendor == null)
            {
                return false;
            }

            await _vendorRepository.DeleteAsync(id);
            return true;
        }
    }
}