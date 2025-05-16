using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Repositories;

namespace tenis_pro_back.Controllers
{
    public class AuthController : Controller
    {
        private readonly IProfile _profileRepository;
        private readonly IUser _userRepository;
        private readonly IConfiguration _config;
        private readonly IUserActivationToken _userActivationToken;
        private readonly EmailHelper _emailHelper;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IProfile profileRepository, IUser userRepository, IUserActivationToken userActivationToken, EmailHelper emailHelper, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _userActivationToken = userActivationToken;
            _emailHelper = emailHelper;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
        }

        public class LoginDto
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }


        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string token)
        {
            try
            {
                var activation = await _userActivationToken.GetByToken(token);

                if (activation == null || activation.Expiration < DateTime.UtcNow)
                    return BadRequest("Token inválido o expirado.");

                await _userActivationToken.Delete(activation.Token);

                User? user = await _userRepository.GetById(activation.UserId);

                if (user == null)
                    return BadRequest("Usuario no encontrado.");

                user.Status = UserStatus.Enabled;

                await _userRepository.Put(user.Id!, user);

                return Ok("Cuenta activada. Ahora puedes iniciar sesión.");
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpGet("initialize")]
        public async Task<IActionResult> Initialize()
        {
            try
            {
                string? EmailAdmin = _config["EMailAdmin"];

                Profile? profileAdmin = await _profileRepository.GetByType( Profile.ProfileType.Admin);

                if (profileAdmin == null)
                {
                    Profile newProfile = new Profile()
                    {
                        Functionalities = new List<string>(),
                        Name = "Admin",
                        Type = Profile.ProfileType.Admin
                    };

                    profileAdmin = await _profileRepository.Post(newProfile);

                    if (String.IsNullOrEmpty(EmailAdmin)) throw new ApplicationException("EmailAdmin not found.");

                    User? userAdmin = await _userRepository.GetByProfile(profileAdmin.Id);

                    if(userAdmin == null)
                    {
                        var hasher = new PasswordHasher<object>();
                        string password = "Peter998!";
                        string hashedPassword = hasher.HashPassword(null, password);

                        User newUser = new User()
                        {
                            LastName = "Admin",
                            Name = "Admin",
                            ProfileId = profileAdmin.Id,
                            Status= UserStatus.Enabled,
                            Email = EmailAdmin,
                            Password = hashedPassword
                        };

                        await _userRepository.Post(newUser);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                if (await _userRepository.GetByEmail(user.Email)!=null)
                    return BadRequest("Email ya está registrado.");

                if (String.IsNullOrEmpty(user.Password))
                    return BadRequest("No se puede dar de alta un usuario sin contraseña.");

                var hasher = new PasswordHasher<object>();
                string passwordHashed = hasher.HashPassword(null, user.Password);

                Profile? profilePlayer = await _profileRepository.GetByType(Profile.ProfileType.Players);


                User newUser = new User()
                {
                    LastName = "Admin",
                    Name = "Admin",
                    ProfileId = profilePlayer.Id,
                    Status = UserStatus.PendingActivation,
                    Email = user.Email,
                    Password = passwordHashed,
                    BirthDate = user.BirthDate,
                    CategoryId = user.CategoryId,
                    Comment = user.Comment,
                    
                };

                user = await _userRepository.Post(user);

                // Crear token de activación
                var token = new UserActivationToken
                {
                    UserId = user.Id,
                    Token = Guid.NewGuid().ToString(),
                    Expiration =  DateTime.UtcNow.AddHours(24),
                    User = user
                };

                await _userActivationToken.Post(token);

                var request = _httpContextAccessor.HttpContext?.Request;

                if (request != null)
                {
                    var scheme = request.Scheme;           // http o https
                    var host = request.Host.Value;         // localhost:5000 o dominio
                    var activationUrl = $"{scheme}://{host}/activate?token={token.Token}";
                }
                
                // Simula envío de email (puedes usar MailKit, SendGrid, etc.)
                await _emailHelper.SendAsync(user.Email, "Activa tu cuenta", $"Haz clic aquí para activar tu cuenta: {activationUrl}");

                return Ok("Usuario registrado. Revisa tu correo para activar la cuenta.");
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmail(dto.Email);

                if (user == null)
                    return Unauthorized("Usuario inexistente.");

                var hasher = new PasswordHasher<object>();
                string passwordHashed = hasher.HashPassword(null, dto.Password);

                if (dto.Password != user.Password)
                    return Unauthorized("Credenciales inválidas.");

                if (user.Status != UserStatus.Enabled)
                    return Unauthorized("Cuenta no activada.");


                var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.ProfileId);

                // Configuramos las opciones para la cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,     // No accesible desde JavaScript
                    Secure = true,       // Solo para HTTPS
                    SameSite = SameSiteMode.Strict,  // Política de SameSite (evita CSRF)
                    Expires = DateTime.UtcNow.AddDays(7) // Duración de la cookie (7 días en este caso)
                };

                // Enviamos el token como una cookie HttpOnly
                Response.Cookies.Append("jwt", token, cookieOptions);


                return Ok(new { token });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
    }
}
