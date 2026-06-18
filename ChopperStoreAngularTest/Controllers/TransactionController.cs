using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using ChopperStoreAngularTest.Models.Dtos;
using ChopperStoreAngularTest.Services;

namespace ChopperStoreAngularTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ChopperStoreContext _context;
        private readonly IMercadoPagoService _mercadoPagoService;

        public TransactionController(ChopperStoreContext context, IMercadoPagoService mercadoPagoService)
        {
            _context = context;
            _mercadoPagoService = mercadoPagoService;
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
                transactionDate = DateTime.UtcNow,
                PaymentStatus = "pending"
            };

            await _context.transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            string checkoutUrl;
            try
            {
                var (preferenceId, initPoint) = await _mercadoPagoService.CreatePreferenceAsync(transaction, cart.items);
                transaction.MercadoPagoPreferenceId = preferenceId;
                checkoutUrl = initPoint;
            }
            catch (Exception)
            {
                _context.transactions.Remove(transaction);
                await _context.SaveChangesAsync();

                return StatusCode(502, new Response<CheckoutResultDto>
                {
                    IsSuccess = false,
                    Message = "No se pudo iniciar el pago con Mercado Pago. Intentá de nuevo en unos minutos.",
                    Result = null
                });
            }

            _context.items.RemoveRange(cart.items);
            await _context.SaveChangesAsync();

            return Ok(new Response<CheckoutResultDto>
            {
                IsSuccess = true,
                Message = "Compra creada, falta confirmar el pago",
                Result = new CheckoutResultDto
                {
                    TransactionId = transaction.Id,
                    CheckoutUrl = checkoutUrl
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("webhook/mercadopago")]
        public async Task<IActionResult> MercadoPagoWebhook([FromQuery] string? topic, [FromQuery] string? id, [FromBody] MercadoPagoWebhookDto? body)
        {
            var paymentIdRaw = body?.Data?.Id ?? (string.Equals(topic, "payment", StringComparison.OrdinalIgnoreCase) ? id : null);

            if (string.IsNullOrEmpty(paymentIdRaw) || !long.TryParse(paymentIdRaw, out var paymentId))
            {
                return Ok();
            }

            MercadoPago.Resource.Payment.Payment payment;
            try
            {
                payment = await _mercadoPagoService.GetPaymentAsync(paymentId);
            }
            catch (Exception)
            {
                return Ok();
            }

            if (payment?.ExternalReference == null || !int.TryParse(payment.ExternalReference, out var transactionId))
            {
                return Ok();
            }

            var transaction = await _context.transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
            if (transaction == null)
            {
                return Ok();
            }

            transaction.MercadoPagoPaymentId = paymentId.ToString();
            transaction.PaymentStatus = payment.Status switch
            {
                "approved" => "approved",
                "rejected" or "cancelled" => "rejected",
                _ => "pending"
            };

            await _context.SaveChangesAsync();

            return Ok();
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
