using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace tenis_pro_back.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IProfile _profileRepository;
        private readonly IUser _userRepository;
        private readonly IUserToken _userTokenRepository;
        private readonly IConfiguration _config;
        private readonly EmailHelper _emailHelper;
        private readonly EncryptionHelper _encryptionHelper;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IProfile profileRepository, IUser userRepository, EmailHelper emailHelper, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper, IConfiguration config, EncryptionHelper encryptionHelper, IUserToken userTokenRepository)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _emailHelper = emailHelper;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
            _config = config;
            _encryptionHelper = encryptionHelper;
            _userTokenRepository = userTokenRepository;
        }

        [AllowAnonymous]
        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string token)
        {
            try
            {
                string? userId = _jwtHelper.GetUserIdFromToken(token);

                if (userId == null) return Unauthorized();

                User? user = await _userRepository.GetById(userId);

                if (user == null)
                    return BadRequest("Usuario no encontrado.");

                

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
        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize()
        {
            try
            {
                string? EmailAdmin = _config["EMail:Admin"];

                Profile? profileAdmin = await _profileRepository.GetByType(Profile.ProfileType.Admin);

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

                    if (userAdmin == null)
                    {
                        var hasher = new PasswordHasher<object>();
                        string hashedPassword = "AQAAAAIAAYagAAAAEN2aNG67QNjn+MQUW8LTyXF0lMmBRcX0Aw0cbBOGEXEWfkTv87B28M…";
                        //string hashedPassword = hasher.HashPassword(null, password);

                        User newUser = new User()
                        {
                            LastName = "Admin",
                            Name = "Admin",
                            ProfileId = profileAdmin.Id,
                            Status = UserStatus.Enabled,
                            Email = EmailAdmin,
                            Password = hashedPassword
                        };

                        await _userRepository.Post(newUser);
                    }

                    //Agrega los demas perfiles (sacar si hago abm
                    newProfile = new Profile()
                    {
                        Functionalities = new List<string>(),
                        Name = "Jugador",
                        Type = Profile.ProfileType.Players
                    };

                    await _profileRepository.Post(newProfile);

                    newProfile = new Profile()
                    {
                        Functionalities = new List<string>(),
                        Name = "Organizador",
                        Type = Profile.ProfileType.Organizer
                    };

                    await _profileRepository.Post(newProfile);

                }

                return Ok(new
                {
                    Message = "Inicialización completada correctamente.",
                    AdminCreated = true,
                    ProfilesCreated = true
                });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { error = ex.Message });

            }
        }

        [AllowAnonymous]
        [HttpPost("ResentActivationEmail")]
        public async Task<IActionResult> ResentActivationEmail([FromBody] User user)
        {
            try
            {
                Int32 expirationTime = 60; //default
                var JwtExpirationTime = _config["JwtExpirationTime:Activation"];

                if (JwtExpirationTime != null)
                    expirationTime = Int32.Parse(JwtExpirationTime);


                User? oUser = await _userRepository.GetById(user.Id);

                if (oUser == null)
                    throw new ApplicationException($"El usuario con email {user.Email} , no se encuentra registrado");


                var token = _jwtHelper.GenerateToken(user, expirationTime);

                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { error = ex.Message });

            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
        {
            try
            {
                if (await _userRepository.GetByEmail(user.Email) != null)
                    return BadRequest("Email ya está registrado.");

                Profile? profilePlayer = await _profileRepository.GetByType(Profile.ProfileType.Players);

                if (profilePlayer == null) throw new ApplicationException("No se puede encontrar el perfil [Jugador].");

                var hasher = new PasswordHasher<object>();
                var hashedPassword = hasher.HashPassword(null, user.Password);


                User newUser = new User()
                {
                    LastName = user.LastName,
                    Name = user.Name,
                    ProfileId = profilePlayer.Id,
                    Status = UserStatus.PendingActivation,
                    Email = user.Email,
                    BirthDate = user.BirthDate,
                    CategoryId = user.CategoryId,
                    Password = hashedPassword,
                    Image = user.Image
                };

                newUser = await _userRepository.Post(newUser);

                // Crear token de activación
                Int32 expirationTime = 60; //default

                var JwtExpirationTime = _config["JwtExpirationTime:Activation"];

                if (JwtExpirationTime != null)
                    expirationTime = Int32.Parse(JwtExpirationTime);


                var token = _jwtHelper.GenerateToken(newUser, expirationTime);

                await SendActivationEmail(token, user.Email);

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { error = ex.Message });

            }
        }

        private async Task SendActivationEmail(string token, string email)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            var activationUrl = "";

            if (request != null)
            {
                var scheme = request.Scheme;           // http o https
                var host = request.Host.Value;         // localhost:5000 o dominio
                activationUrl = $"{scheme}://{host}/api/auth/activate?token={token}";
            }



            // Simula envío de email (puedes usar MailKit, SendGrid, etc.)
            await _emailHelper.SendAsync(email, "Activa tu cuenta", $"Haz clic aquí para activar tu cuenta: {activationUrl}");


        }

        private async Task SendResetPasswordEmail(string token, ResetPasswordDto request)
        {
            var resetPasswordUrl = $"{request.RedirectUrl}?token={token}";


            string htmlBody = $@"
                <p>Hacé clic en el siguiente botón para cambiar tu contraseña:</p>
                <a href='{resetPasswordUrl}'
                   style='display:inline-block;padding:12px 20px;background-color:#1976d2;color:white;text-decoration:none;border-radius:5px;font-weight:bold;'>
                   Cambiar Contraseña
                </a>
                <p>Si no solicitaste este cambio, podés ignorar este mensaje.</p>
            ";


            // Simula envío de email (puedes usar MailKit, SendGrid, etc.)
            await _emailHelper.SendAsync(request.Email, "Reestablecimiento de contraseña", htmlBody);


        }


        [AllowAnonymous]
        [HttpPut("ValidateResetPasswordToken")]
        public async Task<IActionResult> ValidateResetPasswordToken([FromQuery]Guid token)
        {
            try
            {
                var userToken = await _userTokenRepository.GetTokenAsync(token);

                if (userToken == null || userToken.Used)
                    return BadRequest(new { success = false, message = "Token inválido o ya utilizado." });

                if (userToken.ExpirationDate < DateTime.UtcNow)
                    return BadRequest(new { success = false, message = "Token expirado." });


                //Vuelve a buscar el usuario por si se elimino durante la vigencia del jwt token
                User? user = await _userRepository.GetById(userToken.UserId);

                if (user == null)
                    return BadRequest("Usuario no encontrado.");

                user.Password = null;
                user.Status = UserStatus.ChangePassword;

                await _userRepository.Put(user.Id!, user);

                return Ok(new { success = true, userId = user.Id });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                User? user = await _userRepository.GetById(request.Id);

                if (user == null)
                    return Ok(new { success = false, message = "Usuario no encontrado." });

                var hasher = new PasswordHasher<object>();
                var hashedPassword = hasher.HashPassword(null, request.Password);


                user.Password = hashedPassword;
                user.Status = UserStatus.Enabled;

                await _userRepository.Put(request.Id, user);

                return Ok(new { success=true});
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpPut("ResetPasswordRequest")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordDto request)
        {
            try
            {
                int expirationMinutes = 15;

                var expirationConfig = _config["TokenExpirationMinutes:ResetPassword"];
                if (!string.IsNullOrEmpty(expirationConfig))
                    expirationMinutes = int.Parse(expirationConfig);

                Models.User? user = await _userRepository.GetByEmail(request.Email);

                if (user == null)
                    return Ok(new { success = false, message= "Email no registrado." });


                // Crear token
                UserToken userToken = new UserToken
                {                    
                    UserId = user.Id,
                    Token = Guid.NewGuid(),
                    ExpirationDate = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    Used = false
                };

                // Guardar en MongoDB
                await _userTokenRepository.CreateAsync(userToken);

                await SendResetPasswordEmail(userToken.Token.ToString(), request);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmail(dto.Email);

                if (user == null)
                    return Unauthorized(new { errorCode = Models.Enums.AuthErrorEnum.UserNotFound, errorDescription = "Usuario inexistente." });


                var hasher = new PasswordHasher<object>();
                var result = hasher.VerifyHashedPassword(null, user.Password??"", dto.Password);

                if (result != PasswordVerificationResult.Success)
                    return Unauthorized(new { errorCode = Models.Enums.AuthErrorEnum.InvalidCredentials, errorDescription = "Credenciales inválidas." });

                if (user.Status == UserStatus.Disabled)
                    return Unauthorized(new { errorCode = Models.Enums.AuthErrorEnum.UserDisabled, errorDescription = "Cuenta deshabilitada.", userId = user.Id });


                if (user.Status == UserStatus.PendingActivation)
                    return Unauthorized(new { errorCode = Models.Enums.AuthErrorEnum.UnactivatedUser, errorDescription = "Cuenta no activada.", userId = user.Id });
                                
                var token = _jwtHelper.GenerateToken(user,60);

                // Configuramos las opciones para la cookie
                //var cookieOptions = new CookieOptions
                //{
                //    HttpOnly = true,     // No accesible desde JavaScript
                //    Secure = true,       // Solo para HTTPS
                //    SameSite = SameSiteMode.Strict,  // Política de SameSite (evita CSRF)
                //    Expires = DateTime.UtcNow.AddDays(7) // Duración de la cookie (7 días en este caso)
                //};

                //// Enviamos el token como una cookie HttpOnly
                //Response.Cookies.Append("jwt", token, cookieOptions);


                return Ok(new { token });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { error = ex.Message });

            }
        }
    }
}
