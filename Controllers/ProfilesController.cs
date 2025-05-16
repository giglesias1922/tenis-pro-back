using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController: ControllerBase
    {
        private readonly IProfile _profilesRepository;

        public ProfilesController(IProfile profilesRepository)
        {
            _profilesRepository = profilesRepository;
        }

        // GET: api/profiles
        [HttpGet]
        public async Task<ActionResult<List<Profile>>> Get()
        {
            try
            {
                var list = await _profilesRepository.GetAll();
                return Ok(list.OrderBy(o => o.Name));
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // GET: api/profiles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> Get(string id)
        {
            try
            {
                var obj = await _profilesRepository.GetById(id);
                if (obj == null) return NotFound();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // POST: api/profiles
        [HttpPost]
        public async Task<ActionResult> Create(Profile obj)
        {
            try
            {
                await _profilesRepository.Post(obj);
                return CreatedAtAction(nameof(Get), new { id = obj.Id }, obj);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // PUT: api/profiles/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Profile obj)
        {
            try
            {
                var existingObj = await _profilesRepository.GetById(id);
                if (existingObj == null) return NotFound();

                await _profilesRepository.Put(id, obj);
                return NoContent();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // DELETE: api/profiles/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var existingObj = await _profilesRepository.GetById(id);
                if (existingObj == null) return NotFound();


                await _profilesRepository.Delete(id);
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