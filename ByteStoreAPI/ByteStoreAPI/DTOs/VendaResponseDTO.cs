namespace ByteStoreAPI.DTOs
{
    public class VendaResponseDTO
    {
        public Guid Id { get; set; }
        public string PedidoId { get; set; }
        public string Customer { get; set; }
        public decimal Value { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime DataDaVenda { get; set; }
        public List<VendaItemResponseDTO> Items { get; set; }
    }

    public class VendaItemResponseDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
    }
}