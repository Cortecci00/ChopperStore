using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using ChopperStoreAngularTest.Models.Dtos;

namespace ChopperStoreAngularTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ChopperStoreContext _context;

        public ShoppingCartController(ChopperStoreContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private async Task<ShoppingCart> GetOrCreateCartAsync()
        {
            var cart = await _context.shoppingcarts
                .Include(c => c.items).ThenInclude(i => i.skin)
                .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = CurrentUserId };
                await _context.shoppingcarts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var cart = await GetOrCreateCartAsync();

            return Ok(new Response<ShoppingCart>
            {
                IsSuccess = true,
                Message = "Carrito actual",
                Result = cart
            });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<AddCartItemDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var skin = await _context.skins.FindAsync(model.SkinId);
            if (skin == null)
            {
                return BadRequest(new Response<AddCartItemDto>
                {
                    IsSuccess = false,
                    Message = $"No existe una skin con el id {model.SkinId}",
                    Result = model
                });
            }

            var cart = await GetOrCreateCartAsync();
            var item = cart.items.FirstOrDefault(i => i.SkinId == model.SkinId);

            if (item != null)
            {
                item.quantity += model.Quantity;
            }
            else
            {
                item = new Item { SkinId = model.SkinId, quantity = model.Quantity, ShoppingCartId = cart.Id };
                await _context.items.AddAsync(item);
            }

            await _context.SaveChangesAsync();

            return Ok(new Response<ShoppingCart>
            {
                IsSuccess = true,
                Message = "Item agregado al carrito",
                Result = await GetOrCreateCartAsync()
            });
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItem(int itemId, [FromBody] UpdateCartItemDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<UpdateCartItemDto>
                {
                    IsSuccess = false,
                    Message = "La cantidad no es válida",
                    Result = model
                });
            }

            var cart = await GetOrCreateCartAsync();
            var item = cart.items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return NotFound(new Response<UpdateCartItemDto>
                {
                    IsSuccess = false,
                    Message = "No se encontró el item en tu carrito",
                    Result = model
                });
            }

            item.quantity = model.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new Response<ShoppingCart>
            {
                IsSuccess = true,
                Message = "Item actualizado",
                Result = cart
            });
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var cart = await GetOrCreateCartAsync();
            var item = cart.items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return NotFound(new Response<ShoppingCart>
                {
                    IsSuccess = false,
                    Message = "No se encontró el item en tu carrito",
                    Result = null
                });
            }

            _context.items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new Response<ShoppingCart>
            {
                IsSuccess = true,
                Message = "Item eliminado del carrito",
                Result = cart
            });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var cart = await GetOrCreateCartAsync();
            _context.items.RemoveRange(cart.items);
            await _context.SaveChangesAsync();

            return Ok(new Response<string>
            {
                IsSuccess = true,
                Message = "Carrito vaciado",
                Result = null
            });
        }
    }
}
