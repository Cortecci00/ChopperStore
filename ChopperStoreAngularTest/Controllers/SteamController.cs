using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using ChopperStoreAngularTest.Models.Dtos;
using ChopperStoreAngularTest.Services;

namespace ChopperStoreAngularTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class SteamController : ControllerBase
    {
        private readonly ChopperStoreContext _context;
        private readonly ISteamInventoryService _steamInventoryService;

        public SteamController(ChopperStoreContext context, ISteamInventoryService steamInventoryService)
        {
            _context = context;
            _steamInventoryService = steamInventoryService;
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventory()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _context.users.FirstOrDefaultAsync(u => u.Id == int.Parse(idClaim!));

            if (string.IsNullOrEmpty(usuario?.SteamTradeUrl))
            {
                return BadRequest(new Response<List<SteamInventoryItemDto>>
                {
                    IsSuccess = false,
                    Message = "Primero configurá tu Trade URL de Steam en tu perfil",
                    Result = null
                });
            }

            try
            {
                var items = await _steamInventoryService.GetInventoryAsync(usuario.SteamTradeUrl);
                return Ok(new Response<List<SteamInventoryItemDto>>
                {
                    IsSuccess = true,
                    Message = "Inventario obtenido correctamente",
                    Result = items
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new Response<List<SteamInventoryItemDto>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = null
                });
            }
        }
    }
}
