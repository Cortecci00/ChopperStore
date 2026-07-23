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
    public class UsersController : ControllerBase
    {
        private readonly ChopperStoreContext _context;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UsersController(ChopperStoreContext context, IUserService userService, IGoogleAuthService googleAuthService, IAuthService authService, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _googleAuthService = googleAuthService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            if (string.IsNullOrEmpty(request?.Token))
            {
                return BadRequest("No se recibió token");
            }

            var payload = await _googleAuthService.VerifyGoogleTokenAsync(request.Token);
            if (payload == null)
            {
                return Unauthorized("Token de Google inválido");
            }

            var existingUser = await _userService.GetUserByGoogleIdAsync(payload.Subject)
                                ?? await _userService.GetUserByEmailAsync(payload.Email);

            if (existingUser == null)
            {
                var newUser = new User
                {
                    GoogleId = payload.Subject,
                    email = payload.Email,
                    username = payload.Email,
                    name = payload.GivenName,
                    lastname = payload.FamilyName,
                    isAdmin = false,
                    isBlocked = false,
                    password = null
                };

                existingUser = await _userService.CreateUserAsync(newUser);
            }

            var token = _authService.GenerateToken(existingUser);

            return Ok(new Response<AuthResponseDto>
            {
                IsSuccess = true,
                Message = "Inicio de sesión con Google exitoso",
                Result = new AuthResponseDto { User = existingUser, Token = token }
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _userService.GetUserByEmailAsync(request.email);
            if (user == null || string.IsNullOrEmpty(user.password) || !BCrypt.Net.BCrypt.Verify(request.password, user.password))
            {
                return Unauthorized("Email o contraseña incorrectos");
            }

            var token = _authService.GenerateToken(user);

            return Ok(new Response<AuthResponseDto>
            {
                IsSuccess = true,
                Message = "Inicio de sesión exitoso",
                Result = new AuthResponseDto { User = user, Token = token }
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUpdate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<CreateUpdate>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = model
                });
            }

            var existeUsuario = await _context.users.AnyAsync(u => u.email == model.email);
            if (existeUsuario)
            {
                return BadRequest(new Response<CreateUpdate>
                {
                    IsSuccess = false,
                    Message = "El email ya está registrado",
                    Result = model
                });
            }

            var nuevoUsuario = new User
            {
                email = model.email,
                username = model.username,
                password = BCrypt.Net.BCrypt.HashPassword(model.password),
                isAdmin = false,
                isBlocked = false
            };

            await _context.users.AddAsync(nuevoUsuario);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateToken(nuevoUsuario);

            return Ok(new Response<AuthResponseDto>
            {
                IsSuccess = true,
                Message = "Usuario registrado correctamente",
                Result = new AuthResponseDto { User = nuevoUsuario, Token = token }
            });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    IsSuccess = false,
                    Message = "El email no es válido",
                    Result = null
                });
            }

            var usuario = await _context.users.FirstOrDefaultAsync(u => u.email == model.Email);
            if (usuario != null)
            {
                usuario.PasswordResetToken = Guid.NewGuid().ToString("N");
                usuario.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                var frontendUrl = _configuration["App:FrontendUrl"];
                var resetLink = $"{frontendUrl}/reset-password?token={usuario.PasswordResetToken}";
                await _emailService.SendPasswordResetEmailAsync(usuario.email!, resetLink);
            }

            return Ok(new Response<string>
            {
                IsSuccess = true,
                Message = "Si el email está registrado, te enviamos instrucciones para recuperar tu contraseña",
                Result = null
            });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    IsSuccess = false,
                    Message = "Datos inválidos",
                    Result = null
                });
            }

            var usuario = await _context.users.FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token);
            if (usuario == null || usuario.PasswordResetTokenExpiry == null || usuario.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new Response<string>
                {
                    IsSuccess = false,
                    Message = "El link es inválido o expiró, solicitá uno nuevo",
                    Result = null
                });
            }

            usuario.password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            usuario.PasswordResetToken = null;
            usuario.PasswordResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return Ok(new Response<string>
            {
                IsSuccess = true,
                Message = "Contraseña actualizada correctamente",
                Result = null
            });
        }

        [AllowAnonymous]
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

            var existeUsuario = await _context.users.AnyAsync(u => u.email == model.email);
            if (existeUsuario)
            {
                return BadRequest(new Response<CreateUpdate>
                {
                    IsSuccess = false,
                    Message = "El email ya está registrado",
                    Result = model
                });
            }

            var usuarioNuevo = new User
            {
                email = model.email,
                username = model.username,
                password = BCrypt.Net.BCrypt.HashPassword(model.password)
            };

            await _context.users.AddAsync(usuarioNuevo);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateToken(usuarioNuevo);

            return Ok(new Response<AuthResponseDto>
            {
                IsSuccess = true,
                Result = new AuthResponseDto { User = usuarioNuevo, Token = token },
                Message = "Usuario creado correctamente"
            });
        }

        [Authorize]
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

        [Authorize]
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

            if (!EsPropietarioOAdmin(id))
            {
                return Forbid();
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

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateProfileDto model)
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

            if (!EsPropietarioOAdmin(id))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<User>
                {
                    IsSuccess = false,
                    Message = "No se puede actualizar",
                    Result = null
                });
            }

            var usuario = await GetUsers(id);
            if (usuario == null)
            {
                return NotFound(new Response<User>
                {
                    IsSuccess = false,
                    Message = $"No se encontró un usuario con el id {id}",
                    Result = null
                });
            }

            if (model.email != null && model.email != usuario.email)
            {
                var emailEnUso = await _context.users.AnyAsync(u => u.email == model.email && u.Id != id);
                if (emailEnUso)
                {
                    return BadRequest(new Response<User>
                    {
                        IsSuccess = false,
                        Message = "El email ya está registrado por otro usuario",
                        Result = null
                    });
                }
                usuario.email = model.email;
            }

            if (model.name != null) usuario.name = model.name;
            if (model.lastname != null) usuario.lastname = model.lastname;
            if (model.username != null) usuario.username = model.username;
            if (model.phone != null) usuario.phone = model.phone;

            await _context.SaveChangesAsync();

            return Ok(new Response<User>
            {
                IsSuccess = true,
                Message = "Usuario actualizado",
                Result = usuario
            });
        }

        [Authorize]
        [HttpPut("{id}/photo")]
        public async Task<IActionResult> PutPhoto(int id, [FromBody] UpdatePhotoDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<User>
                {
                    IsSuccess = false,
                    Message = "La foto es obligatoria",
                    Result = null
                });
            }

            if (!EsPropietarioOAdmin(id))
            {
                return Forbid();
            }

            var usuario = await GetUsers(id);
            if (usuario == null)
            {
                return NotFound(new Response<User>
                {
                    IsSuccess = false,
                    Message = $"No se encontró un usuario con el id {id}",
                    Result = null
                });
            }

            usuario.PhotoUrl = model.PhotoUrl;
            await _context.SaveChangesAsync();

            return Ok(new Response<User>
            {
                IsSuccess = true,
                Message = "Foto actualizada",
                Result = usuario
            });
        }

        [Authorize]
        [HttpPut("{id}/steam-trade-url")]
        public async Task<IActionResult> PutSteamTradeUrl(int id, [FromBody] UpdateSteamTradeUrlDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<User>
                {
                    IsSuccess = false,
                    Message = "La Trade URL es obligatoria",
                    Result = null
                });
            }

            if (!EsPropietarioOAdmin(id))
            {
                return Forbid();
            }

            var usuario = await GetUsers(id);
            if (usuario == null)
            {
                return NotFound(new Response<User>
                {
                    IsSuccess = false,
                    Message = $"No se encontró un usuario con el id {id}",
                    Result = null
                });
            }

            if (SteamInventoryService.ParseTradeUrlToSteamId64(model.SteamTradeUrl) == null)
            {
                return BadRequest(new Response<User>
                {
                    IsSuccess = false,
                    Message = "La Trade URL no es válida",
                    Result = null
                });
            }

            usuario.SteamTradeUrl = model.SteamTradeUrl;
            await _context.SaveChangesAsync();

            return Ok(new Response<User>
            {
                IsSuccess = true,
                Message = "Trade URL actualizada",
                Result = usuario
            });
        }

        private bool EsPropietarioOAdmin(int id)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdminClaim = User.FindFirstValue("isAdmin");
            return isAdminClaim == "true" || idClaim == id.ToString();
        }

        private async Task<User> GetUsers(int id)
        {
            var usuario = await _context.users.FirstOrDefaultAsync(x => x.Id == id);
            return usuario;
        }
    }
}
