namespace ByteStoreAPI.DTOs
{
    public class VendorProductListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Image { get; set; } 
        public decimal Price { get; set; }
        public bool Active { get; set; }
    }
}