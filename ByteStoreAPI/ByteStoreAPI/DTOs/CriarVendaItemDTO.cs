namespace ByteStoreAPI.DTOs
{
    public class CriarVendaItemDTO
    {
        public Guid ProductId { get; set; }
        public Guid ProductVariationId { get; set; }
        public int Quantidade { get; set; }
    }
}