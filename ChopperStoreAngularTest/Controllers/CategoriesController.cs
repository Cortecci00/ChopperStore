using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using ChopperStoreAngularTest.Models.Dtos;

namespace ChopperStoreAngularTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ChopperStoreContext _context;

        public CategoriesController(ChopperStoreContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _context.categories.ToListAsync();
            return Ok(new Response<IEnumerable<Category>>
            {
                IsSuccess = true,
                Message = "Listado de categorías",
                Result = categorias
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _context.categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound(new Response<Category>
                {
                    IsSuccess = false,
                    Message = "No se encontró la categoría",
                    Result = null
                });
            }

            return Ok(new Response<Category>
            {
                IsSuccess = true,
                Message = "Se encontró la categoría",
                Result = categoria
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<CategoryCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var categoria = new Category { name = model.name };
            await _context.categories.AddAsync(categoria);
            await _context.SaveChangesAsync();

            return Ok(new Response<Category>
            {
                IsSuccess = true,
                Message = "Categoría creada correctamente",
                Result = categoria
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<CategoryCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var categoria = await _context.categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound(new Response<CategoryCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = $"No se encontró una categoría con el id {id}",
                    Result = model
                });
            }

            categoria.name = model.name;
            await _context.SaveChangesAsync();

            return Ok(new Response<Category>
            {
                IsSuccess = true,
                Message = "Categoría actualizada",
                Result = categoria
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound(new Response<Category>
                {
                    IsSuccess = false,
                    Message = "No se encontró la categoría",
                    Result = null
                });
            }

            var tieneSkins = await _context.skins.AnyAsync(s => s.category.Id == id);
            if (tieneSkins)
            {
                return BadRequest(new Response<Category>
                {
                    IsSuccess = false,
                    Message = "No se puede eliminar la categoría porque tiene skins asociadas",
                    Result = categoria
                });
            }

            _context.categories.Remove(categoria);
            await _context.SaveChangesAsync();

            return Ok(new Response<Category>
            {
                IsSuccess = true,
                Message = "Categoría eliminada",
                Result = categoria
            });
        }
    }
}
