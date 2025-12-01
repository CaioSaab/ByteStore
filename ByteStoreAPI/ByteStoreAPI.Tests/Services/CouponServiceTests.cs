using Xunit;
using Moq;
using FluentAssertions;
using ByteStoreAPI.Service;
using ByteStoreAPI.Interfaces;
using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Tests.Services
{
    public class CouponServiceTests
    {
        private readonly Mock<ICouponRepository> _mockRepository;
        private readonly CouponService _couponService;

        public CouponServiceTests()
        {
            _mockRepository = new Mock<ICouponRepository>();
            _couponService = new CouponService(_mockRepository.Object);
        }

        [Fact]
        public async Task ValidateAsync_DeveRetornarCouponDTOQuandoCodigoEhValido()
        {
            // Arrange
            var codigo = "DESCONTO10";
            var cupom = new Cupom
            {
                Code = codigo,
                DiscountPercentage = 10,
                IsActive = true
            };

            _mockRepository.Setup(r => r.GetActiveByCodeAsync(codigo))
                .ReturnsAsync(cupom);

            // Act
            var result = await _couponService.ValidateAsync(codigo);

            // Assert
            result.Should().NotBeNull();
            result!.Code.Should().Be(codigo);
            result.DiscountPercentage.Should().Be(10);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_DeveRetornarNullQuandoCodigoNaoExiste()
        {
            // Arrange
            var codigo = "INVALIDO";
            _mockRepository.Setup(r => r.GetActiveByCodeAsync(codigo))
                .ReturnsAsync((Cupom?)null);

            // Act
            var result = await _couponService.ValidateAsync(codigo);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetVendorCouponsAsync_DeveRetornarListaDeCupons()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var cupons = new List<Cupom>
            {
                new Cupom { Code = "CUPOM1", DiscountPercentage = 10, IsActive = true },
                new Cupom { Code = "CUPOM2", DiscountPercentage = 20, IsActive = false }
            };

            _mockRepository.Setup(r => r.GetByVendorIdAsync(vendorId))
                .ReturnsAsync(cupons);

            // Act
            var result = await _couponService.GetVendorCouponsAsync(vendorId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Code.Should().Be("CUPOM1");
            result[1].Code.Should().Be("CUPOM2");
        }

        [Fact]
        public async Task UpsertAsync_DeveCriarNovoCoupon()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var dto = new UpsertCouponDTO
            {
                Code = "NOVO10",
                DiscountPercentage = 10,
                IsActive = true
            };

            var cupom = new Cupom
            {
                Code = dto.Code,
                DiscountPercentage = dto.DiscountPercentage,
                IsActive = dto.IsActive
            };

            _mockRepository.Setup(r => r.UpsertAsync(vendorId, dto.Code, dto.DiscountPercentage, dto.IsActive))
                .ReturnsAsync(cupom);

            // Act
            var result = await _couponService.UpsertAsync(vendorId, dto);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be(dto.Code);
            result.DiscountPercentage.Should().Be(dto.DiscountPercentage);
        }

        [Fact]
        public async Task ToggleActiveAsync_DeveAlternarStatusDoCoupon()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var codigo = "CUPOM1";

            _mockRepository.Setup(r => r.ToggleActiveAsync(vendorId, codigo, true))
                .ReturnsAsync(true);

            // Act
            var result = await _couponService.ToggleActiveAsync(vendorId, codigo, true);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(r => r.ToggleActiveAsync(vendorId, codigo, true), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveDeletarCoupon()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var codigo = "CUPOM1";

            _mockRepository.Setup(r => r.DeleteAsync(vendorId, codigo))
                .ReturnsAsync(true);

            // Act
            var result = await _couponService.DeleteAsync(vendorId, codigo);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(r => r.DeleteAsync(vendorId, codigo), Times.Once);
        }

        [Fact]
        public async Task GetAvailableAsync_DeveRetornarApenasCuponsAtivos()
        {
            // Arrange
            var cupons = new List<Cupom>
            {
                new Cupom { Code = "CUPOM1", DiscountPercentage = 10, IsActive = true },
                new Cupom { Code = "CUPOM2", DiscountPercentage = 20, IsActive = true }
            };

            _mockRepository.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(cupons);

            // Act
            var result = await _couponService.GetAvailableAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.All(c => c.IsActive).Should().BeTrue();
        }
    }
}

