namespace ByteStoreAPI.DTOs
{
    // Usado para deserializar a string JSON 'Data'
    public class CreateProductDataDTO
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Categoria { get; set; }
        public List<CreateVariationDataDTO> Variations { get; set; }
    }

    public class CreateVariationDataDTO
    {
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public string? Cor { get; set; }
        public string? Tamanho { get; set; }

        // Lista de índices dos arquivos (da lista 'Files') que pertencem a esta variação
        // Ex: [0, 1] significa que esta variação usa o primeiro e segundo arquivo da lista 'Files'
        public List<int> FileIndexes { get; set; }
    }
}