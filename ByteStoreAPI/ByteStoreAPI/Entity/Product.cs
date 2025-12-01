using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public enum ProductStatus
    {
        Ativo,
        Inativo
    }

    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Categoria { get; set; }
        public ProductStatus Status { get; set; }
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public List<ProductVariation> Variations { get; set; }
        public Product(Vendor vendor, string nome, string? descricao, string categoria)
        {
            Id = Guid.NewGuid();
            Vendor = vendor;
            VendorId = vendor.Id;
            Nome = nome;
            Descricao = descricao;
            Categoria = categoria;
            Status = ProductStatus.Inativo;
            Variations = new List<ProductVariation>();
        }
        private Product() { }
    }
}