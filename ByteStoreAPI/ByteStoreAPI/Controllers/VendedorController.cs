using ByteStoreAPI.DTOs;
using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace ByteStoreAPI.Controllers
{
    [Route("api/vendedor")]
    [ApiController]
    [Authorize(Roles = "Vendor")]
    public class VendedorController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;
        private readonly IQuestionService _questionService;

        public VendedorController(IProductService productService, ISaleService saleService, IQuestionService questionService)
        {
            _productService = productService;
            _saleService = saleService;
            _questionService = questionService;
        }
        [HttpPost("produtos")]
        public async Task<IActionResult> CreateMyProduct([FromForm] CreateProductFormDTO formDto)
        {
            try
            {
                ModelState.Clear();
                
                if (formDto == null || string.IsNullOrWhiteSpace(formDto.Data))
                {
                    return BadRequest(new { message = "O campo 'Data' (JSON) não foi encontrado ou está vazio." });
                }

                var dto = new CreateProductDTO
                {
                    Data = formDto.Data,
                    Files = formDto.Files
                };

                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var productDto = await _productService.CreateProductAsync(dto, vendorId);
                
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("perguntas")]
        [SwaggerOperation(Summary = "Lista todas as perguntas dos produtos do vendedor logado")]
        public async Task<IActionResult> GetMyQuestions()
        {
            try
            {
                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var perguntas = await _questionService.GetVendorQuestionsAsync(vendorId);
                return Ok(perguntas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar perguntas.", error = ex.Message });
            }
        }

        public class AnswerRequest { public string Answer { get; set; } }

        [HttpPost("perguntas/{id}/responder")]
        [SwaggerOperation(Summary = "Responde uma pergunta de um produto do vendedor logado")]
        public async Task<IActionResult> AnswerQuestion(Guid id, [FromBody] AnswerRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.Answer))
                    return BadRequest(new { message = "Resposta não pode ser vazia." });

                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _questionService.AnswerQuestionAsync(id, vendorId, req.Answer);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao responder pergunta.", error = ex.Message });
            }
        }
        [HttpGet("produtos")]
        [SwaggerOperation(Summary = "Lista todos os produtos do vendedor logado")]
        public async Task<IActionResult> GetMyProducts()
        {
            try
            {
                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var products = await _productService.GetProductsByVendorIdAsync(vendorId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar produtos.", error = ex.Message });
            }
        }

        [HttpPatch("produtos/{id}/status")]
        [SwaggerOperation(Summary = "Ativa ou desativa um produto")]
        public async Task<IActionResult> ToggleMyProductStatus(Guid id, [FromBody] UpdateStatusDTO dto)
        {
            try
            {
                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _productService.SetProductStatusAsync(id, vendorId, dto.Active);
                return NoContent(); // Sucesso, sem conteúdo
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // 403 Proibido
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("produtos/{id}")]
        public async Task<IActionResult> DeleteMyProduct(Guid id)
        {
            try
            {
                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var success = await _productService.DeleteProductAsync(id, vendorId);

                if (!success)
                {
                    return NotFound(new { message = "Produto não encontrado." });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class UpdateStockRequest { public int Estoque { get; set; } }

        [HttpPatch("produtos/{productId}/variacoes/{variationId}/estoque")]
        public async Task<IActionResult> UpdateVariationStock(Guid productId, Guid variationId, [FromBody] UpdateStockRequest req)
        {
            try
            {
                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _productService.UpdateVariationStockAsync(productId, vendorId, variationId, req.Estoque);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("vendas")]
        public async Task<IActionResult> GetMinhasVendas()
        {
            try
            {
                var vendedorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var vendasDto = await _saleService.GetSalesForSellerAsync(vendedorId);
                return Ok(vendasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar suas vendas.", error = ex.Message });
            }
        }

        public class UpdateSaleStatusRequest { public int Status { get; set; } }

        [HttpPatch("vendas/{id}/status")]
        public async Task<IActionResult> UpdateSaleStatus(Guid id, [FromBody] UpdateSaleStatusRequest req)
        {
            try
            {
                var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = (OrderStatus)req.Status;
                var ok = await _saleService.UpdateSaleStatusAsync(id, vendorId, status);
                if (!ok) return BadRequest(new { message = "Não foi possível atualizar o status." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
