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
using Google.Apis.Auth;
using ChopperStoreAngularTest.Services;

namespace ChopperStoreAngularTest.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ChopperStoreContext _context;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserService _userService;

        public UsersController(ChopperStoreContext context, IUserService userService, IGoogleAuthService googleAuthService)
        {
            _context = context;
            _userService = userService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            if (string.IsNullOrEmpty(request?.Token))
            {
                return BadRequest("Token de Google no recibido");
            }

            var googleUser = await _googleAuthService.VerifyGoogleTokenAsync(request.Token);
            if (googleUser == null)
            {
                return BadRequest("Token de Google inválido");
            }

            var user = await _userService.GetUserByGoogleIdAsync(googleUser.Subject);

            if (user == null)
            {
                // Si el usuario no existe, lo creamos
                user = new User
                {
                    GoogleId = googleUser.Subject,
                    email = googleUser.Email,
                    name = googleUser.Name,
                    password = Guid.NewGuid().ToString() // Generar una contraseña de forma automática
                };

                user = await _userService.CreateUserAsync(user);
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _userService.GetUserByEmailAsync(request.email);
            if (user == null || user.password != request.password)
            {
                return Unauthorized("Email o contraseña incorrectos");
            }
            return Ok(user);
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
                email = model.email,
                username = model.username,
                password = model.password
            };

            await _context.users.AddAsync(usuarioNuevo);
            await _context.SaveChangesAsync();

            return Ok(new Response<User>
            {
                IsSuccess = true,
                Result = usuarioNuevo,
                Message = "Usuario creado correctamente"
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

                usuario.email = model.email;
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
