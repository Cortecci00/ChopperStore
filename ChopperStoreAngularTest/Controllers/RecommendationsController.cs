using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using ChopperStoreAngularTest.Models.Dtos;

namespace ChopperStoreAngularTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly ChopperStoreContext _context;

        public RecommendationsController(ChopperStoreContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin => User.FindFirstValue("isAdmin") == "true";

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var recomendaciones = await _context.recommendations.Include(r => r.usuario).ToListAsync();
            return Ok(new Response<IEnumerable<Recommendation>>
            {
                IsSuccess = true,
                Message = "Listado de recomendaciones",
                Result = recomendaciones
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecommendationCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<RecommendationCreateDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var usuario = await _context.users.FindAsync(CurrentUserId);
            if (usuario == null)
            {
                return Unauthorized();
            }

            var recomendacion = new Recommendation { usuario = usuario, text = model.Text };
            await _context.recommendations.AddAsync(recomendacion);
            await _context.SaveChangesAsync();

            return Ok(new Response<Recommendation>
            {
                IsSuccess = true,
                Message = "Recomendación creada correctamente",
                Result = recomendacion
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var recomendacion = await _context.recommendations.Include(r => r.usuario).FirstOrDefaultAsync(r => r.Id == id);
            if (recomendacion == null)
            {
                return NotFound(new Response<Recommendation>
                {
                    IsSuccess = false,
                    Message = "No se encontró la recomendación",
                    Result = null
                });
            }

            if (recomendacion.usuario.Id != CurrentUserId && !IsAdmin)
            {
                return Forbid();
            }

            _context.recommendations.Remove(recomendacion);
            await _context.SaveChangesAsync();

            return Ok(new Response<Recommendation>
            {
                IsSuccess = true,
                Message = "Recomendación eliminada",
                Result = recomendacion
            });
        }
    }
}
