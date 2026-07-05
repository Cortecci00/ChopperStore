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
    public class SkinsController : ControllerBase
    {
        private readonly ChopperStoreContext _context;
        private readonly ISkinStockService _skinStockService;

        public SkinsController(ChopperStoreContext context, ISkinStockService skinStockService)
        {
            _context = context;
            _skinStockService = skinStockService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool storefrontOnly = false)
        {
            var query = _context.skins.Include(s => s.category).AsQueryable();

            if (storefrontOnly)
            {
                var reservedSkinIds = await _skinStockService.GetReservedSkinIdsAsync();
                query = query.Where(s => !reservedSkinIds.Contains(s.Id));
            }

            var skins = await query.ToListAsync();
            return Ok(new Response<IEnumerable<Skin>>
            {
                IsSuccess = true,
                Message = "Listado de skins",
                Result = skins
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var skin = await _context.skins.Include(s => s.category).FirstOrDefaultAsync(s => s.Id == id);
            if (skin == null)
            {
                return NotFound(new Response<Skin>
                {
                    IsSuccess = false,
                    Message = "No se encontró la skin",
                    Result = null
                });
            }

            return Ok(new Response<Skin>
            {
                IsSuccess = true,
                Message = "Se encontró la skin",
                Result = skin
            });
        }

        [AllowAnonymous]
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] bool storefrontOnly = false)
        {
            var query = _context.skins.Include(s => s.category)
                .Where(s => s.category.Id == categoryId);

            if (storefrontOnly)
            {
                var reservedSkinIds = await _skinStockService.GetReservedSkinIdsAsync();
                query = query.Where(s => !reservedSkinIds.Contains(s.Id));
            }

            var skins = await query.ToListAsync();

            return Ok(new Response<IEnumerable<Skin>>
            {
                IsSuccess = true,
                Message = "Listado de skins por categoría",
                Result = skins
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkinCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<SkinCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var categoria = await _context.categories.FindAsync(model.categoryId);
            if (categoria == null)
            {
                return BadRequest(new Response<SkinCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = $"No existe una categoría con el id {model.categoryId}",
                    Result = model
                });
            }

            var skin = new Skin
            {
                name = model.name,
                skinFloat = model.skinFloat,
                pattern = model.pattern,
                rarity = model.rarity,
                price = model.price,
                category = categoria,
                PhotoUrl = model.photoUrl,
                InspectLink = model.inspectLink
            };

            await _context.skins.AddAsync(skin);
            await _context.SaveChangesAsync();

            return Ok(new Response<Skin>
            {
                IsSuccess = true,
                Message = "Skin creada correctamente",
                Result = skin
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SkinCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<SkinCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var skin = await _context.skins.FirstOrDefaultAsync(s => s.Id == id);
            if (skin == null)
            {
                return NotFound(new Response<SkinCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = $"No se encontró una skin con el id {id}",
                    Result = model
                });
            }

            var categoria = await _context.categories.FindAsync(model.categoryId);
            if (categoria == null)
            {
                return BadRequest(new Response<SkinCreateUpdateDto>
                {
                    IsSuccess = false,
                    Message = $"No existe una categoría con el id {model.categoryId}",
                    Result = model
                });
            }

            skin.name = model.name;
            skin.skinFloat = model.skinFloat;
            skin.pattern = model.pattern;
            skin.rarity = model.rarity;
            skin.price = model.price;
            skin.category = categoria;
            if (model.photoUrl != null) skin.PhotoUrl = model.photoUrl;
            skin.InspectLink = model.inspectLink;

            await _context.SaveChangesAsync();

            return Ok(new Response<Skin>
            {
                IsSuccess = true,
                Message = "Skin actualizada",
                Result = skin
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skin = await _context.skins.FirstOrDefaultAsync(s => s.Id == id);
            if (skin == null)
            {
                return NotFound(new Response<Skin>
                {
                    IsSuccess = false,
                    Message = "No se encontró la skin",
                    Result = null
                });
            }

            await _skinStockService.RemoveSkinAsync(id);

            return Ok(new Response<Skin>
            {
                IsSuccess = true,
                Message = "Skin eliminada",
                Result = skin
            });
        }
    }
}
