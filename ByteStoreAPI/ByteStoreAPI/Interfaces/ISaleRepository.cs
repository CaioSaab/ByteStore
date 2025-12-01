using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface ISaleRepository
    {
        Task AddSaleAsync(Sale sale);
        Task<List<SaleItem>> GetSaleItemsByVendedorIdAsync(Guid vendedorId);
        Task<List<Sale>> GetSalesByCompradorIdAsync(Guid compradorId);
    }
}