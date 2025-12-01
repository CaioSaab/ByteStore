using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }

        public Guid ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; }

        public ProductImage(string imageUrl)
        {
            Id = Guid.NewGuid();
            ImageUrl = imageUrl;
        }
        private ProductImage() { } // Para EF Core
    }
}