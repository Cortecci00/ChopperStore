using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;

namespace ChopperStoreAngularTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ChopperStoreContext _context;

        public TransactionController(ChopperStoreContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin => User.FindFirstValue("isAdmin") == "true";

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _context.shoppingcarts
                .Include(c => c.items).ThenInclude(i => i.skin)
                .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

            if (cart == null || cart.items.Count == 0)
            {
                return BadRequest(new Response<string>
                {
                    IsSuccess = false,
                    Message = "El carrito está vacío",
                    Result = null
                });
            }

            var transactionItems = cart.items.Select(i => new TransactionItem
            {
                SkinId = i.SkinId,
                quantity = i.quantity,
                unitPriceAtPurchase = i.skin.price
            }).ToList();

            var transaction = new Transaction
            {
                UserId = CurrentUserId,
                items = transactionItems,
                totalPrice = transactionItems.Sum(ti => ti.unitPriceAtPurchase * ti.quantity),
                transactionDate = DateTime.UtcNow
            };

            await _context.transactions.AddAsync(transaction);
            _context.items.RemoveRange(cart.items);
            await _context.SaveChangesAsync();

            return Ok(new Response<Transaction>
            {
                IsSuccess = true,
                Message = "Compra realizada correctamente",
                Result = transaction
            });
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var transacciones = await _context.transactions
                .Include(t => t.items).ThenInclude(ti => ti.skin)
                .Where(t => t.UserId == CurrentUserId)
                .OrderByDescending(t => t.transactionDate)
                .ToListAsync();

            return Ok(new Response<IEnumerable<Transaction>>
            {
                IsSuccess = true,
                Message = "Tus compras",
                Result = transacciones
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaccion = await _context.transactions
                .Include(t => t.items).ThenInclude(ti => ti.skin)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaccion == null)
            {
                return NotFound(new Response<Transaction>
                {
                    IsSuccess = false,
                    Message = "No se encontró la transacción",
                    Result = null
                });
            }

            if (transaccion.UserId != CurrentUserId && !IsAdmin)
            {
                return Forbid();
            }

            return Ok(new Response<Transaction>
            {
                IsSuccess = true,
                Message = "Detalle de la transacción",
                Result = transaccion
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transacciones = await _context.transactions
                .Include(t => t.items).ThenInclude(ti => ti.skin)
                .OrderByDescending(t => t.transactionDate)
                .ToListAsync();

            return Ok(new Response<IEnumerable<Transaction>>
            {
                IsSuccess = true,
                Message = "Listado de todas las transacciones",
                Result = transacciones
            });
        }
    }
}
