using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using ChopperStoreAngularTest.Models.Dtos;

namespace ChopperStoreAngularTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ChopperStoreContext _context;

        public ContactController(ChopperStoreContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactMessageCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<ContactMessageCreateDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var mensaje = new ContactMessage
            {
                name = model.name,
                lastname = model.lastname,
                email = model.email,
                message = model.message,
                createdAt = DateTime.UtcNow
            };

            await _context.contactMessages.AddAsync(mensaje);
            await _context.SaveChangesAsync();

            return Ok(new Response<ContactMessage>
            {
                IsSuccess = true,
                Message = "Mensaje enviado correctamente",
                Result = mensaje
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var mensajes = await _context.contactMessages
                .OrderByDescending(m => m.createdAt)
                .ToListAsync();

            return Ok(new Response<IEnumerable<ContactMessage>>
            {
                IsSuccess = true,
                Message = "Listado de mensajes de contacto",
                Result = mensajes
            });
        }
    }
}
