using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ChopperStoreAngularTest.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ChopperStoreContext _context;

        public UsersController(ChopperStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var usuarios = await _context.users.ToListAsync();
            if (usuarios.Any())
            {
                return Ok(new Response<IEnumerable<User>>
                {
                    IsSuccess = false,
                    Result = usuarios,
                    Message = "Listado de usuarios"
                });
            }

            return Ok(new Response<IEnumerable<User>>
            {
                IsSuccess = false,
                Message = "No hay registros para mostrar",
                Result = []
            });
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUpdate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<CreateUpdate>
                {
                    IsSuccess = false,
                    Result = model,
                    Message = "Los campos no son correctos"
                });
            }
            var usuarioNuevo = new User
            {
                name = model.name,
                lastname = model.lastname,
                email = model.email,
                phone = model.phone,
                username = model.username,
                password = model.password,
                isAdmin = model.isAdmin,
                isBlocked = model.isBlocked

            };
            await _context.users.AddAsync(usuarioNuevo);
            await _context.SaveChangesAsync();

            return Ok(new Response<User>
            {
                IsSuccess = true,
                Result = usuarioNuevo,
                Message = "Usuario creado correctamente",
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new Response<User>
                {
                    IsSuccess = false,
                    Message = "El id es necesario",
                    Result = null
                });
            }

            var usuario = await GetUsers(id);
            if (usuario != null)
            {
                return Ok(new Response<User>
                {
                    IsSuccess = true,
                    Message = "Se encontró un usuario",
                    Result = usuario,
                });
            }
            return NotFound(new Response<User>
            {
                IsSuccess = false,
                Message = "No hay coincidencias",
                Result = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new Response<User>
                {
                    IsSuccess = false,
                    Message = "El id no es correcto",
                    Result = null
                });
            }
            var usuario = await GetUsers(id);
            if (usuario != null)
            {
                _context.users.Remove(usuario);
                await _context.SaveChangesAsync();

                return Ok(new Response<User>
                {
                    Result = usuario,
                    IsSuccess = true,
                    Message = $"Se ha eliminado el usuario {usuario.name}"
                });
            }
            return NotFound(new Response<User>
            {
                IsSuccess = false,
                Message = "No hay coincidencias",
                Result = usuario
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateUpdate model)
        { 
            if (id <= 0) 
            { 
                return BadRequest(new Response<CreateUpdate> 
                {
                    IsSuccess = false,
                    Message = "El id es necesario",
                    Result = model
                });
            }
            if (ModelState.IsValid) 
            {
                var usuario = await GetUsers(id);
                if (usuario == null)
                {
                    return NotFound(new Response<CreateUpdate>
                    {
                        IsSuccess = false,
                        Message = $"No se encontró un usuario con el id {id}",
                        Result = model
                    });
                }

                usuario.name = model.name;
                usuario.lastname = model.lastname;
                usuario.email = model.email;
                usuario.phone = model.phone;
                usuario.username = model.username;
                usuario.password = model.password;

                _context.users.Update(usuario);
                await _context.SaveChangesAsync();
                
                return Ok(new Response<CreateUpdate>
                {
                    IsSuccess = true,
                    Message = "Usuario actualizado",
                    Result= model
                });
            }
            return BadRequest(new Response<CreateUpdate>
            { 
                IsSuccess= false,
                Message= "No se puede actualizar",
                Result= model
            });

        }

        private async Task<User> GetUsers(int id) 
        { 
            var usuario = await _context.users.FirstOrDefaultAsync(x => x.Id == id);
            return usuario;
        }



    }
}
