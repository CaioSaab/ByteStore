namespace ByteStoreAPI.DTOs
{
    public class VendorQuestionDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ClientName { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
        public DateTime AskedAt { get; set; }
    }
}
