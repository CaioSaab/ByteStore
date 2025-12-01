using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class ProductVariation
    {
        [Key]
        public Guid Id { get; set; }
        
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public string? Cor { get; set; }
        public string? Tamanho { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public List<ProductImage> Imagens { get; set; }
        public ProductVariation(Product product, decimal preco, int estoque, string? cor, string? tamanho)
        {
            Id = Guid.NewGuid();
            Product = product;
            ProductId = product.Id;
            Preco = preco;
            Estoque = estoque;
            Cor = cor;
            Tamanho = tamanho;
            Imagens = new List<ProductImage>();
        }
        private ProductVariation() { }
    }
}