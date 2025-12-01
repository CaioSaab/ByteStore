using ByteStoreAPI.DTOs;
using ByteStoreAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByteStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IClientService _clientService;

        public AccountController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var contaDto = await _clientService.GetById(id);
            if (contaDto == null)
            {
                return NotFound();
            }
            return Ok(contaDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var contasDto = await _clientService.GetAllClients();
            return Ok(contasDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateAccountDTO dto)
        {
            var contaAtualizadaDto = await _clientService.UpdateClient(id, dto);
            if (contaAtualizadaDto == null)
            {
                return NotFound();
            }
            return Ok(contaAtualizadaDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var success = await _clientService.DeleteClient(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}