namespace ByteStoreAPI.DTOs
{
    public class UpsertCouponDTO
    {
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}