using ByteStoreAPI.Entity;

namespace ByteStoreAPI.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Pergunta> AddQuestionAsync(Guid productId, Guid clientId, string conteudo);
        Task<List<Pergunta>> GetQuestionsByProductIdAsync(Guid productId);
        Task<List<Pergunta>> GetQuestionsByVendorIdAsync(Guid vendorId);
        Task<Pergunta?> GetByIdAsync(Guid perguntaId);
        Task<Resposta> AddAnswerAsync(Guid perguntaId, Guid vendorId, string conteudo);
    }
}