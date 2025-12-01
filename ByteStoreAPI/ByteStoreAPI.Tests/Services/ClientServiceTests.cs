using Xunit;
using Moq;
using FluentAssertions;
using ByteStoreAPI.Service;
using ByteStoreAPI.Interfaces;
using ByteStoreAPI.Services;
using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using Microsoft.Extensions.Configuration;

namespace ByteStoreAPI.Tests.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _mockRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly ClientService _clientService;

        public ClientServiceTests()
        {
            _mockRepository = new Mock<IClientRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _clientService = new ClientService(_mockRepository.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task CreateClient_DeveCriarClienteComSucesso()
        {
            // Arrange
            var dto = new CreateAccountDTO
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Password = "senha123"
            };

            var novoCliente = new Client(dto.Nome, dto.Email, "hash_password");
            novoCliente.Id = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Client?)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Client>()))
                .ReturnsAsync(novoCliente);

            // Act
            var result = await _clientService.CreateClient(dto);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(dto.Nome);
            result.Email.Should().Be(dto.Email);
            result.Id.Should().Be(novoCliente.Id);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task CreateClient_DeveLancarExcecaoQuandoEmailJaExiste()
        {
            // Arrange
            var dto = new CreateAccountDTO
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Password = "senha123"
            };

            var clienteExistente = new Client("João", dto.Email, "hash");
            _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(clienteExistente);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _clientService.CreateClient(dto));
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarTokenQuandoCredenciaisSaoValidas()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "joao@example.com",
                Password = "senha123"
            };

            var cliente = new Client("João", dto.Email, BCrypt.Net.BCrypt.HashPassword(dto.Password));
            cliente.Id = Guid.NewGuid();
            var tokenEsperado = "token_jwt_aqui";

            _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(cliente);
            _mockTokenService.Setup(t => t.GenerateToken(cliente.Id, cliente.Email, cliente.Nome, "Client"))
                .Returns(tokenEsperado);

            // Act
            var result = await _clientService.LoginAsync(dto);

            // Assert
            result.Should().Be(tokenEsperado);
            _mockTokenService.Verify(t => t.GenerateToken(cliente.Id, cliente.Email, cliente.Nome, "Client"), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecaoQuandoEmailNaoExiste()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "inexistente@example.com",
                Password = "senha123"
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Client?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _clientService.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecaoQuandoSenhaEhInvalida()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "joao@example.com",
                Password = "senha_errada"
            };

            var cliente = new Client("João", dto.Email, BCrypt.Net.BCrypt.HashPassword("senha_correta"));
            _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(cliente);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _clientService.LoginAsync(dto));
        }

        [Fact]
        public async Task GetById_DeveRetornarClienteQuandoEncontrado()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Client("João", "joao@example.com", "hash");
            cliente.Id = clienteId;

            _mockRepository.Setup(r => r.GetByIdAsync(clienteId))
                .ReturnsAsync(cliente);

            // Act
            var result = await _clientService.GetById(clienteId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(clienteId);
            result.Nome.Should().Be("João");
            result.Email.Should().Be("joao@example.com");
        }

        [Fact]
        public async Task GetById_DeveRetornarNullQuandoClienteNaoEncontrado()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(clienteId))
                .ReturnsAsync((Client?)null);

            // Act
            var result = await _clientService.GetById(clienteId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateClient_DeveAtualizarClienteComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Client("João", "joao@example.com", "hash");
            cliente.Id = clienteId;

            var dto = new UpdateAccountDTO
            {
                Nome = "João Silva",
                Email = "joao.silva@example.com"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(clienteId))
                .ReturnsAsync(cliente);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _clientService.UpdateClient(clienteId, dto);

            // Assert
            result.Should().NotBeNull();
            result!.Nome.Should().Be(dto.Nome);
            result.Email.Should().Be(dto.Email);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task DeleteClient_DeveRetornarTrueQuandoClienteEhDeletado()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Client("João", "joao@example.com", "hash");
            cliente.Id = clienteId;

            _mockRepository.Setup(r => r.GetByIdAsync(clienteId))
                .ReturnsAsync(cliente);
            _mockRepository.Setup(r => r.DeleteAsync(clienteId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _clientService.DeleteClient(clienteId);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(r => r.DeleteAsync(clienteId), Times.Once);
        }

        [Fact]
        public async Task DeleteClient_DeveRetornarFalseQuandoClienteNaoEncontrado()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(clienteId))
                .ReturnsAsync((Client?)null);

            // Act
            var result = await _clientService.DeleteClient(clienteId);

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}

