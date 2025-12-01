using ByteStoreAPI.Data;
using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Service
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ByteStoreDbContext _dbContext;

        public SaleService(ISaleRepository saleRepository, ByteStoreDbContext dbContext)
        {
            _saleRepository = saleRepository;
            _dbContext = dbContext;
        }

        public async Task<bool> CreateSaleAsync(CriarVendaDTO dto, Guid compradorId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                decimal precoTotalVenda = 0;
                var sale = new Sale(compradorId, 0);
                await _dbContext.Sales.AddAsync(sale);

                foreach (var itemDto in dto.Itens)
                {
                    var variation = await _dbContext.ProductVariations
                        .Include(pv => pv.Product)
                        .FirstOrDefaultAsync(pv => pv.Id == itemDto.ProductVariationId);

                    if (variation == null)
                        throw new InvalidOperationException($"Variação de produto com ID {itemDto.ProductVariationId} não encontrada.");

                    if (variation.Estoque < itemDto.Quantidade)
                        throw new InvalidOperationException($"Estoque insuficiente para {variation.Product.Nome} (Cor: {variation.Cor}, Tamanho: {variation.Tamanho}).");

                    variation.Estoque -= itemDto.Quantidade;
                    _dbContext.ProductVariations.Update(variation);

                    var precoUnitario = variation.Preco;
                    precoTotalVenda += precoUnitario * itemDto.Quantidade;

                    var saleItem = new SaleItem(sale.Id, variation.Id, itemDto.Quantidade, precoUnitario);
                    await _dbContext.SaleItems.AddAsync(saleItem);
                }

                decimal discount = 0;
                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    var coupon = await _dbContext.Cupons.FirstOrDefaultAsync(c => c.IsActive && c.Code == dto.CouponCode.ToUpper());
                    if (coupon != null)
                    {
                        discount = precoTotalVenda * (coupon.DiscountPercentage / 100m);
                    }
                }

                sale.PrecoTotal = precoTotalVenda - discount + dto.ShippingCost;
                sale.PaymentMethod = dto.PaymentMethod ?? string.Empty;
                sale.Status = OrderStatus.Preparando;

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<VendaResponseDTO>> GetSalesForSellerAsync(Guid vendedorId)
        {
            var saleItems = await _saleRepository.GetSaleItemsByVendedorIdAsync(vendedorId);

            var salesGrouped = saleItems.GroupBy(item => item.Sale);

            var responseList = new List<VendaResponseDTO>();

            foreach (var saleGroup in salesGrouped)
            {
                var sale = saleGroup.Key;
                var itemsInThisGroup = saleGroup.ToList(); 

                responseList.Add(new VendaResponseDTO
                {
                    Id = sale.Id,
                    PedidoId = "#" + sale.Id.ToString().Substring(0, 4).ToUpper(),
                    Customer = sale.Comprador.Nome, 
                    Value = sale.PrecoTotal,
                    Status = sale.Status == OrderStatus.Enviado ? "Enviado" : "A Enviar",
                    PaymentMethod = sale.PaymentMethod,
                    DataDaVenda = sale.DataDaCompra,
                    Items = itemsInThisGroup.Select(item => new VendaItemResponseDTO
                    {
                        Name = item.ProductVariation.Product.Nome,
                        Quantity = item.QuantidadeComprada,
                        Color = item.ProductVariation.Cor,
                        Size = item.ProductVariation.Tamanho
                    }).ToList()
                });
            }

            return responseList;
        }

        public async Task<List<VendaResponseDTO>> GetSalesForBuyerAsync(Guid compradorId)
        {
            var sales = await _saleRepository.GetSalesByCompradorIdAsync(compradorId);

            var responseList = new List<VendaResponseDTO>();

            foreach (var sale in sales)
            {
                responseList.Add(new VendaResponseDTO
                {
                    Id = sale.Id,
                    PedidoId = "#" + sale.Id.ToString().Substring(0, 4).ToUpper(),
                    Customer = sale.Comprador.Nome,
                    Value = sale.PrecoTotal,
                    Status = sale.Status == OrderStatus.Enviado ? "Enviado" : "A Enviar",
                    PaymentMethod = sale.PaymentMethod,
                    DataDaVenda = sale.DataDaCompra,
                    Items = sale.Itens.Select(item => new VendaItemResponseDTO
                    {
                        Name = item.ProductVariation.Product.Nome,
                        Quantity = item.QuantidadeComprada,
                        Color = item.ProductVariation.Cor,
                        Size = item.ProductVariation.Tamanho
                    }).ToList()
                });
            }

            return responseList;
        }
        public async Task<bool> UpdateSaleStatusAsync(Guid saleId, Guid vendorId, OrderStatus status)
        {
            if (status != OrderStatus.Preparando && status != OrderStatus.Enviado) return false;

            var sale = await _dbContext.Sales
                .Include(s => s.Itens)
                    .ThenInclude(i => i.ProductVariation)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) return false;

            var hasVendorItem = sale.Itens.Any(i => i.ProductVariation.Product.VendorId == vendorId);
            if (!hasVendorItem) return false;

            sale.Status = status;
            _dbContext.Sales.Update(sale);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}