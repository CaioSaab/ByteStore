using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using ByteStoreAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ByteStoreAPI.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<QuestionDTO> AddQuestionAsync(Guid productId, Guid clientId, string conteudo)
        {
            var pergunta = await _questionRepository.AddQuestionAsync(productId, clientId, conteudo);
            return new QuestionDTO
            {
                Id = pergunta.Id,
                Question = pergunta.Conteudo,
                Answer = pergunta.Resposta?.Conteudo,
                AskedAt = pergunta.CriadaEm
            };
        }

        public async Task<List<QuestionDTO>> GetProductQuestionsAsync(Guid productId)
        {
            var perguntas = await _questionRepository.GetQuestionsByProductIdAsync(productId);
            return perguntas.Select(p => new QuestionDTO
            {
                Id = p.Id,
                Question = p.Conteudo,
                Answer = p.Resposta?.Conteudo,
                AskedAt = p.CriadaEm
            }).ToList();
        }

        public async Task<List<VendorQuestionDTO>> GetVendorQuestionsAsync(Guid vendorId)
        {
            var perguntas = await _questionRepository.GetQuestionsByVendorIdAsync(vendorId);
            return perguntas.Select(p => new VendorQuestionDTO
            {
                Id = p.Id,
                ProductId = p.Produto.Id,
                ProductName = p.Produto.Nome,
                ClientName = p.Conta.Nome,
                Question = p.Conteudo,
                Answer = p.Resposta?.Conteudo,
                AskedAt = p.CriadaEm
            }).ToList();
        }

        public async Task<QuestionDTO> AnswerQuestionAsync(Guid perguntaId, Guid vendorId, string conteudo)
        {
            var resposta = await _questionRepository.AddAnswerAsync(perguntaId, vendorId, conteudo);
            var pergunta = await _questionRepository.GetByIdAsync(perguntaId)
                ?? throw new KeyNotFoundException("Pergunta não encontrada.");

            return new QuestionDTO
            {
                Id = pergunta.Id,
                Question = pergunta.Conteudo,
                Answer = resposta.Conteudo,
                AskedAt = pergunta.CriadaEm
            };
        }
    }
}