using ByteStoreAPI.Data;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ByteStoreDbContext _dbContext;

        public ProductRepository(ByteStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _dbContext.Produtos.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Produtos.FindAsync(id);
        }

        public async Task<List<Product>> GetByVendorIdAsync(Guid vendorId)
        {
            return await _dbContext.Produtos
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Imagens)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Produtos
                .Include(p => p.Vendor)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Imagens)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(List<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Produtos
                .Include(p => p.Vendor)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Imagens)
                .AsQueryable();

            query = query.Where(p => p.Status == ProductStatus.Ativo);

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Produtos.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _dbContext.Produtos.FindAsync(id);
            if (product != null)
            {
                _dbContext.Produtos.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}