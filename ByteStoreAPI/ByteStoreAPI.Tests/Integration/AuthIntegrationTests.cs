using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using ByteStoreAPI.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace ByteStoreAPI.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegistrarComprador_DeveRetornarSucesso()
        {
            // Arrange
            var dto = new CreateAccountDTO
            {
                Nome = "Teste User",
                Email = $"teste_{Guid.NewGuid()}@example.com",
                Password = "senha123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/RegistrarComprador", dto);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        [Fact]
        public async Task RegistrarComprador_DeveRetornarErroQuandoEmailJaExiste()
        {
            // Limpa o banco antes do teste
            await ClearDatabaseAsync();

            // Arrange
            var email = $"teste_{Guid.NewGuid()}@example.com";
            var dto = new CreateAccountDTO
            {
                Nome = "Teste User",
                Email = email,
                Password = "senha123"
            };

            // Primeiro registro
            var firstResponse = await _client.PostAsJsonAsync("/api/Auth/RegistrarComprador", dto);
            firstResponse.EnsureSuccessStatusCode(); // Garante que o primeiro registro foi bem-sucedido

            // Segundo registro com mesmo email
            var response = await _client.PostAsJsonAsync("/api/Auth/RegistrarComprador", dto);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
        }

        private async Task ClearDatabaseAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ByteStoreAPI.Data.ByteStoreDbContext>();
                db.Clients.RemoveRange(db.Clients);
                db.Vendors.RemoveRange(db.Vendors);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task LoginComprador_DeveRetornarTokenQuandoCredenciaisValidas()
        {
            // Limpa o banco antes do teste
            await ClearDatabaseAsync();

            // Arrange
            var email = $"login_{Guid.NewGuid()}@example.com";
            var senha = "senha123";

            // Registrar primeiro
            var registroDto = new CreateAccountDTO
            {
                Nome = "Login Test",
                Email = email,
                Password = senha
            };
            var registroResponse = await _client.PostAsJsonAsync("/api/Auth/RegistrarComprador", registroDto);
            registroResponse.EnsureSuccessStatusCode(); // Garante que o registro foi bem-sucedido

            // Fazer login
            var loginDto = new LoginDTO { Email = email, Password = senha };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/LoginComprador", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginComprador_DeveRetornarErroQuandoCredenciaisInvalidas()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "naoexiste@example.com",
                Password = "senhaerrada"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/LoginComprador", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}

