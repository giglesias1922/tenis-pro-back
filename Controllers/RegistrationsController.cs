using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistration _registrationRepository;
        


        public RegistrationsController(IRegistration registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        [HttpGet("{categoryId}/{tournamentId}")]

        public async Task<ActionResult<List<User>>> GetUsersToRegistrationAsync(string categoryId, string tournamentId)
        {
            try
            {
                var data = await _registrationRepository.GetUsersToRegistrationAsync(categoryId, tournamentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
                

        // GET: api
        [HttpGet("tournament/{tournamentId}")]
        public async Task<ActionResult<List<RegistrationsDto>>> Get(string tournamentId)
        {
            try
            {
                var data = await _registrationRepository.GetAll(tournamentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("users/{tournamentId}")]
        public async Task<ActionResult<List<RegistrationUserDto>>> GetRegistratedUsers(string tournamentId)
        {
            try
            {
                var data = await _registrationRepository.GetRegistratedUsers(tournamentId);
                if (data == null) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
        
        

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Registration>> GetById(string id)
        {
            try
            {
                var data = await _registrationRepository.GetById(id);
                if (data == null) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // POST: api/registrations
        [HttpPost]
        public async Task<ActionResult> Create(Registration registration)
        {
            try
            {
                await _registrationRepository.Post(registration);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // DELETE: api/registrations/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var existingCategory = await _registrationRepository.GetById(id);
                if (existingCategory == null) return NotFound();

                await _registrationRepository.Delete(id);
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