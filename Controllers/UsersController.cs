﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUser _userRepository;
        private readonly JwtHelper _jwtHelper;

        public UsersController(IUser userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public class LoginRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }


        // GET: api/users
        [HttpGet]
        [Route("Category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetByCategory(string categoryId)
        {
            try
            {
                var users = await _userRepository.GetByCategory(categoryId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            try
            {
                var user = await _userRepository.GetById(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            try
            {
                User? duplicated = await _userRepository.GetByEmail(user.Email);

                if (duplicated != null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Ya existe un usuario con ese correo."
                    });
                }

                user.Id = null; // Deja que MongoDB genere el Id automáticamente
                await _userRepository.Post(user);

                return Ok(new
                {
                    success = true,
                    data = user
                });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, User user)
        {
            try
            {
                var existingUser = await _userRepository.GetById(id);

                if (existingUser == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Usuario inexistente."
                    });
                }

                User? duplicated = await _userRepository.GetByEmail(user.Email);

                if (duplicated != null && duplicated.Id != id)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Ya existe un usuario con ese correo."
                    });
                }

                user.Password = existingUser.Password;
                await _userRepository.Put(id, user);

                return Ok(new
                {
                    success = true,
                    data = user
                });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }


        
        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userRepository.GetById(id);

                if (user == null)
                {
                    return NotFound();
                }

                await _userRepository.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
    }
}
