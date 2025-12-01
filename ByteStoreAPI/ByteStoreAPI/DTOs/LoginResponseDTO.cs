namespace ByteStoreAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string UserName { get; set; }
    }
}