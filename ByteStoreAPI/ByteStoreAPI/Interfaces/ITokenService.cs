using Microsoft.AspNetCore.Mvc;

namespace ByteStoreAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Guid id, string email, string name, string role);
    }
}
