using ByteStoreAPI.DTOs;

namespace ByteStoreAPI.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionDTO> AddQuestionAsync(Guid productId, Guid clientId, string conteudo);
        Task<List<QuestionDTO>> GetProductQuestionsAsync(Guid productId);
        Task<List<VendorQuestionDTO>> GetVendorQuestionsAsync(Guid vendorId);
        Task<QuestionDTO> AnswerQuestionAsync(Guid perguntaId, Guid vendorId, string conteudo);
    }
}