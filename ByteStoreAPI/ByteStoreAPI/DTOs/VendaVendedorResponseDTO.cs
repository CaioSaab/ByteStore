namespace ByteStoreAPI.DTOs
{
    public class VendaVendedorResponseDTO
    {
        public Guid VendaId { get; set; }
        public DateTime DataDaVenda { get; set; }
        public string ProdutoNome { get; set; }
        public string? Cor { get; set; }
        public string? Tamanho { get; set; }

        public int QuantidadeVendida { get; set; }
        public decimal PrecoUnitarioVendido { get; set; }
        public decimal TotalItemVendido { get; set; }
    }
}