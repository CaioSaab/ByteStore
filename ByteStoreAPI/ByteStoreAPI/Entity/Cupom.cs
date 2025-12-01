using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Cupom
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }
    }
}
