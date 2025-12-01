namespace ByteStoreAPI.DTOs
{
    public class CriarVendaDTO
    {
        public List<ItemVendaDTO> Itens { get; set; }
        public decimal ShippingCost { get; set; }
        public string? CouponCode { get; set; }
        public string PaymentMethod { get; set; }
        public int? Installments { get; set; }
    }

    public class ItemVendaDTO
    {
        public Guid ProductVariationId { get; set; }
        public int Quantidade { get; set; }
    }
}