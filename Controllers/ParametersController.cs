using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Repositories;

namespace tenis_pro_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParametersController:ControllerBase
    {
        private readonly IParameter _parameterRepository;

        public ParametersController(IParameter parameterRepository)
        {
            _parameterRepository = parameterRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parameter>>> GetAll()
        {
            try
            {
                var parameters = await _parameterRepository.GetAll();
                return Ok(parameters);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Parameter>> GetById(string id)
        {
            try
            {
                var parameters = await _parameterRepository.GetById(id);

                if (parameters == null)
                {
                    return NotFound();
                }

                return Ok(parameters);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(Parameter param)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "No se pudo identificar el usuario."
                    });
                }

                Parameter? duplicated = await _parameterRepository.GetById(param.Id);

                if (duplicated != null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Ya existe un parametro con ese id."
                    });
                }

                param.ModifDate = DateTime.UtcNow;
                param.ModifUser = userId;

                await _parameterRepository.Post(param);

                return Ok(new
                {
                    success = true,
                    data = param
                });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Parameter param)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "No se pudo identificar el usuario."
                    });
                }

                var duplicated = await _parameterRepository.GetById(id);

                if (duplicated == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Parametro inexistente."
                    });
                }

                param.ModifDate = DateTime.UtcNow;
                param.ModifUser = userId;
                await _parameterRepository.Put(id, param);

                return Ok(new
                {
                    success = true,
                    data = param
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
                var param = await _parameterRepository.GetById(id);

                if (param == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Parametro inexistente."
                    });
                }

                await _parameterRepository.Delete(id);

                return Ok(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
    }
}
