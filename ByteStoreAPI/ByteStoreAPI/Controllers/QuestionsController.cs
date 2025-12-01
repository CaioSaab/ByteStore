using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ByteStoreAPI.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public class CreateQuestionRequest
        {
            public string Question { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateQuestion(Guid productId, [FromBody] CreateQuestionRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Question))
                return BadRequest(new { message = "Pergunta não pode ser vazia." });

            var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var dto = await _questionService.AddQuestionAsync(productId, clientId, req.Question);
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductQuestions(Guid productId)
        {
            var list = await _questionService.GetProductQuestionsAsync(productId);
            return Ok(new { questions = list });
        }
    }
}