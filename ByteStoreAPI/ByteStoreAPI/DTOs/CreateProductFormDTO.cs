using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.DTOs
{
    public class CreateProductFormDTO
    {
        [FromForm(Name = "Data")]
        [Required(ErrorMessage = "O campo Data é obrigatório")]
        public string Data { get; set; } = string.Empty;

        [FromForm(Name = "Files")]
        public List<IFormFile>? Files { get; set; }
    }
}
