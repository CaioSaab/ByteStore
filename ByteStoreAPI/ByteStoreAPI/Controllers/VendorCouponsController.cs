using ByteStoreAPI.DTOs;
using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ByteStoreAPI.Controllers
{
    [ApiController]
    [Route("api/vendedor/cupons")]
    [Authorize(Roles = "Vendor")]
    public class VendorCouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public VendorCouponsController(ICouponService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await _service.GetVendorCouponsAsync(vendorId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> CriarOuAtualizar([FromBody] UpsertCouponDTO dto)
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var saved = await _service.UpsertAsync(vendorId, dto);
            return Ok(saved);
        }

        public class ToggleCouponRequest { public bool Active { get; set; } }

        [HttpPatch("{code}/status")]
        public async Task<IActionResult> AlterarStatus(string code, [FromBody] ToggleCouponRequest body)
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool active = body.Active;
            var ok = await _service.ToggleActiveAsync(vendorId, code, active);
            if (!ok) return NotFound();
            return Ok(new { code = code.ToUpper(), active });
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Excluir(string code)
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var ok = await _service.DeleteAsync(vendorId, code);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}