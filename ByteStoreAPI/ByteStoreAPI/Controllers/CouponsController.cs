using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByteStoreAPI.Controllers
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponsController(ICouponService service)
        {
            _service = service;
        }

        public class ValidateRequest
        {
            public string Code { get; set; }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateRequest req)
        {
            var c = await _service.ValidateAsync(req.Code);
            if (c == null) return BadRequest(new { message = "Cupom inválido ou expirado" });
            return Ok(new { discountPercentage = c.DiscountPercentage, message = "Cupom aplicado" });
        }

        [HttpGet("available")]
        public async Task<IActionResult> Available()
        {
            var list = await _service.GetAvailableAsync();
            return Ok(list);
        }
    }
}