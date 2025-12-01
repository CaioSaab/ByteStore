using ByteStoreAPI.Data;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ByteStoreAPI.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ByteStoreDbContext _dbContext;

        public QuestionRepository(ByteStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Pergunta> AddQuestionAsync(Guid productId, Guid clientId, string conteudo)
        {
            var product = await _dbContext.Produtos.FindAsync(productId)
                ?? throw new KeyNotFoundException("Produto não encontrado.");
            var client = await _dbContext.Clients.FindAsync(clientId)
                ?? throw new KeyNotFoundException("Cliente não encontrado.");

            var pergunta = new Pergunta(client, DateTime.UtcNow, conteudo, product);
            await _dbContext.Perguntas.AddAsync(pergunta);
            await _dbContext.SaveChangesAsync();
            return pergunta;
        }

        public async Task<List<Pergunta>> GetQuestionsByProductIdAsync(Guid productId)
        {
            return await _dbContext.Perguntas
                .Where(p => p.Produto.Id == productId)
                .Include(p => p.Resposta)
                .OrderByDescending(p => p.CriadaEm)
                .ToListAsync();
        }

        public async Task<List<Pergunta>> GetQuestionsByVendorIdAsync(Guid vendorId)
        {
            return await _dbContext.Perguntas
                .Include(p => p.Produto)
                .Include(p => p.Conta)
                .Include(p => p.Resposta)
                .Where(p => p.Produto.VendorId == vendorId)
                .OrderByDescending(p => p.CriadaEm)
                .ToListAsync();
        }

        public async Task<Pergunta?> GetByIdAsync(Guid perguntaId)
        {
            return await _dbContext.Perguntas
                .Include(p => p.Produto)
                .Include(p => p.Resposta)
                .FirstOrDefaultAsync(p => p.Id == perguntaId);
        }

        public async Task<Resposta> AddAnswerAsync(Guid perguntaId, Guid vendorId, string conteudo)
        {
            var pergunta = await _dbContext.Perguntas
                .Include(p => p.Produto)
                .Include(p => p.Resposta)
                .FirstOrDefaultAsync(p => p.Id == perguntaId)
                ?? throw new KeyNotFoundException("Pergunta não encontrada.");

            if (pergunta.Produto.VendorId != vendorId)
                throw new UnauthorizedAccessException("Você não pode responder perguntas de produtos de outro vendedor.");

            if (pergunta.Resposta != null)
                throw new InvalidOperationException("Pergunta já respondida.");

            var vendor = await _dbContext.Vendors.FindAsync(vendorId)
                ?? throw new KeyNotFoundException("Vendedor não encontrado.");

            var resposta = new Resposta(vendor, DateTime.UtcNow, conteudo, pergunta);
            await _dbContext.Respostas.AddAsync(resposta);
            pergunta.Resposta = resposta;
            await _dbContext.SaveChangesAsync();
            return resposta;
        }
    }
}