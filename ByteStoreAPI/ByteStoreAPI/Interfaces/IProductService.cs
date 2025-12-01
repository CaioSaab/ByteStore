using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto, Guid vendorId);
        Task<ProductResponseDTO?> GetProductByIdAsync(Guid id);
        Task<PageResponse<ProductResponseDTO>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<bool> DeleteProductAsync(Guid productId, Guid vendedorId);
        Task<List<VendorProductListDTO>> GetProductsByVendorIdAsync(Guid vendorId);
        Task SetProductStatusAsync(Guid productId, Guid vendorId, bool newStatus);
        Task<bool> UpdateVariationStockAsync(Guid productId, Guid vendorId, Guid variationId, int estoque);
    }
}
