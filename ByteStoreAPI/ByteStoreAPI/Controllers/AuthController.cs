using ByteStoreAPI.DTOs;
using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByteStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IVendorService _vendorService;

        public AuthController(IClientService clientService, IVendorService vendorService)
        {
            _clientService = clientService;
            _vendorService = vendorService;
        }

        // --- CLIENTE ---

        [HttpPost("LoginComprador")]
        public async Task<IActionResult> LoginBuyer([FromBody] LoginDTO loginDto)
        {
            try
            {
                var token = await _clientService.LoginAsync(loginDto);
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("RegistrarComprador")]
        public async Task<IActionResult> RegisterBuyer([FromBody] CreateAccountDTO dto)
        {
            try
            {
                var novoCliente = await _clientService.CreateClient(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- VENDEDOR ---

        [HttpPost("LoginVendedor")]
        public async Task<IActionResult> LoginSeller([FromBody] LoginDTO loginDto)
        {
            try
            {
                var token = await _vendorService.LoginAsync(loginDto);
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("RegistrarVendedor")]
        public async Task<IActionResult> RegisterSeller([FromBody] CreateAccountDTO dto)
        {
            try
            {
                var novoVendor = await _vendorService.RegisterAsync(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Client")]
        public IActionResult GetCurrentClient()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            var nome = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(idClaim)) return Unauthorized();
            return Ok(new AccountResponseDTO { Id = Guid.Parse(idClaim), Nome = nome, Email = email });
        }

        [HttpGet("vendor/me")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Vendor")]
        public IActionResult GetCurrentVendor()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            var nome = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(idClaim)) return Unauthorized();
            return Ok(new AccountResponseDTO { Id = Guid.Parse(idClaim), Nome = nome, Email = email });
        }
    }
}