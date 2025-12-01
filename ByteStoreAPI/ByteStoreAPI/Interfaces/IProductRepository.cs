using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<List<Product>> GetByVendorIdAsync(Guid vendorId);
        Task<Product?> FindByIdAsync(Guid id); // Um getter simples - toggle
        Task<Product?> GetByIdAsync(Guid id);
        Task<(List<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
