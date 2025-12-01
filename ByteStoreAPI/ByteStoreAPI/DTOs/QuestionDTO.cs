namespace ByteStoreAPI.DTOs
{
    public class QuestionDTO
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
        public DateTime AskedAt { get; set; }
    }
}
