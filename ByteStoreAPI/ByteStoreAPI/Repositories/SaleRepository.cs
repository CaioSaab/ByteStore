using ByteStoreAPI.Data;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ByteStoreDbContext _dbContext;

        public SaleRepository(ByteStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddSaleAsync(Sale sale)
        {
            await _dbContext.Sales.AddAsync(sale);
        }

        public async Task<List<SaleItem>> GetSaleItemsByVendedorIdAsync(Guid vendedorId)
        {
            return await _dbContext.SaleItems
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale.Comprador)

                .Include(item => item.ProductVariation)
                    .ThenInclude(variation => variation.Product)
                .Where(item => item.ProductVariation.Product.VendorId == vendedorId)
                .OrderByDescending(item => item.Sale.DataDaCompra)
                .ToListAsync();
        }

        public async Task<List<Sale>> GetSalesByCompradorIdAsync(Guid compradorId)
        {
            return await _dbContext.Sales
                .Include(s => s.Comprador)
                .Include(s => s.Itens)
                    .ThenInclude(i => i.ProductVariation)
                        .ThenInclude(pv => pv.Product)
                .Where(s => s.CompradorId == compradorId)
                .OrderByDescending(s => s.DataDaCompra)
                .ToListAsync();
        }
    }
}