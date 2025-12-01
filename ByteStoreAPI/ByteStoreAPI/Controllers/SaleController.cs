using ByteStoreAPI.DTOs;
using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ByteStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost]
        public async Task<IActionResult> RealizarCompra([FromBody] CriarVendaDTO dto)
        {
            try
            {
                var compradorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var sucesso = await _saleService.CreateSaleAsync(dto, compradorId);

                if (sucesso)
                {
                    return Ok(new { message = "Compra realizada com sucesso!" });
                }
                return BadRequest("Não foi possível processar a compra.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao processar a compra.");
            }
        }

        [HttpGet("minhas")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMinhasCompras()
        {
            try
            {
                var compradorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var compras = await _saleService.GetSalesForBuyerAsync(compradorId);
                return Ok(compras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar suas compras.", error = ex.Message });
            }
        }
    }
}