using Xunit;
using Moq;
using FluentAssertions;
using ByteStoreAPI.Service;
using ByteStoreAPI.Interfaces;
using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Tests.Services
{
    public class SaleServiceTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly ByteStoreDbContext _dbContext;
        private readonly SaleService _saleService;

        public SaleServiceTests()
        {
            var options = new DbContextOptionsBuilder<ByteStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new ByteStoreDbContext(options);
            _mockSaleRepository = new Mock<ISaleRepository>();
            _saleService = new SaleService(_mockSaleRepository.Object, _dbContext);
        }

        [Fact]
        public async Task CreateSaleAsync_DeveLancarExcecaoQuandoVariacaoNaoExiste()
        {
            // Arrange
            var compradorId = Guid.NewGuid();
            var dto = new CriarVendaDTO
            {
                Itens = new List<ItemVendaDTO>
                {
                    new ItemVendaDTO
                    {
                        ProductVariationId = Guid.NewGuid(),
                        Quantidade = 1
                    }
                },
                ShippingCost = 10m
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _saleService.CreateSaleAsync(dto, compradorId));
        }

        [Fact]
        public async Task CreateSaleAsync_DeveLancarExcecaoQuandoEstoqueInsuficiente()
        {
            // Arrange
            var compradorId = Guid.NewGuid();
            var comprador = new Client("Comprador", "comprador@test.com", "hash");
            var vendor = new Vendor("Vendor", "vendor@test.com", "hash");
            var product = new Product(vendor, "Produto", "Desc", "Cat");
            var variation = new ProductVariation(product, 100m, 5, "Vermelho", "M");
            
            _dbContext.Clients.Add(comprador);
            _dbContext.Vendors.Add(vendor);
            _dbContext.Produtos.Add(product);
            _dbContext.ProductVariations.Add(variation);
            await _dbContext.SaveChangesAsync();

            var dto = new CriarVendaDTO
            {
                Itens = new List<ItemVendaDTO>
                {
                    new ItemVendaDTO
                    {
                        ProductVariationId = variation.Id,
                        Quantidade = 10 // Mais do que o estoque disponível
                    }
                },
                ShippingCost = 10m
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _saleService.CreateSaleAsync(dto, compradorId));
        }

        [Fact]
        public async Task GetSalesForBuyerAsync_DeveRetornarListaDeVendas()
        {
            // Arrange
            var compradorId = Guid.NewGuid();
            var comprador = new Client("Comprador", "comprador@test.com", "hash");
            comprador.Id = compradorId;
            
            var sale = new Sale(compradorId, 100m);
            sale.Comprador = comprador;
            sale.Itens = new List<SaleItem>();
            
            var sales = new List<Sale> { sale };

            _mockSaleRepository.Setup(r => r.GetSalesByCompradorIdAsync(compradorId))
                .ReturnsAsync(sales);

            // Act
            var result = await _saleService.GetSalesForBuyerAsync(compradorId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task UpdateSaleStatusAsync_DeveRetornarFalseQuandoSaleNaoExiste()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var vendorId = Guid.NewGuid();
            var statusValido = OrderStatus.Preparando;

            // Act
            var result = await _saleService.UpdateSaleStatusAsync(saleId, vendorId, statusValido);

            // Assert
            result.Should().BeFalse();
        }
    }
}

