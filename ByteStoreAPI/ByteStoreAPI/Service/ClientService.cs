using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using ByteStoreAPI.Services;

namespace ByteStoreAPI.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ITokenService _tokenService;

        public ClientService(IClientRepository clientRepository, ITokenService tokenService)
        {
            _clientRepository = clientRepository;
            _tokenService = tokenService;
        }

        public async Task<AccountResponseDTO> CreateClient(CreateAccountDTO dto)
        {
            var contaExistente = await _clientRepository.GetByEmailAsync(dto.Email);
            if (contaExistente != null)
            {
                throw new InvalidOperationException("Este e-mail já está em uso.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var novaConta = new Client(dto.Nome, dto.Email, passwordHash);

            var clientAdicionado = await _clientRepository.AddAsync(novaConta);

            return new AccountResponseDTO
            {
                Id = clientAdicionado.Id,
                Nome = clientAdicionado.Nome,
                Email = clientAdicionado.Email
            };
        }

        public async Task<string> LoginAsync(LoginDTO dto)
        {
            var client = await _clientRepository.GetByEmailAsync(dto.Email);

            if (client == null || !BCrypt.Net.BCrypt.Verify(dto.Password, client.PasswordHash))
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            return _tokenService.GenerateToken(client.Id, client.Email, client.Nome, "Client");
        }

        public async Task<AccountResponseDTO?> GetById(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null) return null;

            return new AccountResponseDTO { Id = client.Id, Nome = client.Nome, Email = client.Email };
        }

        public async Task<List<AccountResponseDTO>> GetAllClients()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.Select(c => new AccountResponseDTO { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList();
        }

        public async Task<AccountResponseDTO?> UpdateClient(Guid id, UpdateAccountDTO dto)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return null;
            }

            client.Nome = dto.Nome;
            client.Email = dto.Email;

            await _clientRepository.UpdateAsync(client);
            return new AccountResponseDTO { Id = client.Id, Nome = client.Nome, Email = client.Email };
        }

        public async Task<bool> DeleteClient(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return false;
            }

            await _clientRepository.DeleteAsync(id);
            return true;
        }
    }
}