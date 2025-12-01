namespace ByteStoreAPI.DTOs
{
    public class UpdateProductDTO
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal PrecoBase { get; set; }
        public string ImagemURL { get; set; }
    }
}