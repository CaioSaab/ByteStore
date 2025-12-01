namespace ByteStoreAPI.DTOs
{
    public class ProductResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Categoria { get; set; }
        public Guid VendedorId { get; set; }
        public string VendedorNome { get; set; }
        public List<VariationResponseDTO> Variations { get; set; }
    }

    public class VariationResponseDTO
    {
        public Guid Id { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public string? Cor { get; set; }
        public string? Tamanho { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}