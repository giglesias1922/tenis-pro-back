using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Controllers
{
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
            var data = await _registrationRepository.GetUsersToRegistrationAsync(categoryId,tournamentId);
            return Ok(data);
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
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Registration>> GetById(string id)
        {
            var data = await _registrationRepository.GetById(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        // POST: api/registrations
        [HttpPost]
        public async Task<ActionResult> Create(Registration registration)
        {
            await _registrationRepository.Post(registration);
            return Ok();
        }

        // DELETE: api/registrations/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingCategory = await _registrationRepository.GetById(id);
            if (existingCategory == null) return NotFound();


            await _registrationRepository.Delete(id);
            return NoContent();
        }

    }
}