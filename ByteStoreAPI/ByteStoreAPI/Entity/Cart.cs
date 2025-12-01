using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
    }
}
