using Microsoft.AspNetCore.Http;

namespace ByteStoreAPI.DTOs
{
    public class CreateProductDTO
    {
        public string Data { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}