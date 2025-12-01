using Microsoft.AspNetCore.Http;

namespace ByteStoreAPI.Interfaces
{
    public interface IFileUploadService
    {
        // Salva o arquivo e retorna a URL dele
        Task<string> UploadFileAsync(IFormFile file);
    }
}