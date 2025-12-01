using ByteStoreAPI.DTOs;

namespace ByteStoreAPI.Interfaces
{
    public interface ISaleService
    {
        Task<bool> CreateSaleAsync(CriarVendaDTO dto, Guid compradorId);
        Task<List<VendaResponseDTO>> GetSalesForSellerAsync(Guid vendedorId);
        Task<List<VendaResponseDTO>> GetSalesForBuyerAsync(Guid compradorId);
        Task<bool> UpdateSaleStatusAsync(Guid saleId, Guid vendorId, OrderStatus status);
    }
}