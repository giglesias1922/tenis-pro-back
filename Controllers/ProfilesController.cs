using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tenis_pro_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController: ControllerBase
    {
        private readonly ProfilesRepository _profilesRepository;

        public ProfilesController(ProfilesRepository profilesRepository)
        {
            _profilesRepository = profilesRepository;
        }

        // GET: api/profiles
        [HttpGet]
        public async Task<ActionResult<List<Profile>>> Get()
        {
            var list = await _profilesRepository.GetAllAsync();
            return Ok(list.OrderBy(o => o.Name));
        }

        // GET: api/profiles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> Get(string id)
        {
            var obj = await _profilesRepository.GetByIdAsync(id);
            if (obj == null) return NotFound();
            return Ok(obj);
        }

        // POST: api/profiles
        [HttpPost]
        public async Task<ActionResult> Create(Profile obj)
        {
            await _profilesRepository.CreateAsync(obj);
            return CreatedAtAction(nameof(Get), new { id = obj.Id }, obj);
        }

        // PUT: api/profiles/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Profile obj)
        {
            var existingObj = await _profilesRepository.GetByIdAsync(id);
            if (existingObj == null) return NotFound();

            await _profilesRepository.UpdateAsync(id, obj);
            return NoContent();
        }

        // DELETE: api/profiles/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingObj = await _profilesRepository.GetByIdAsync(id);
            if (existingObj == null) return NotFound();


            await _profilesRepository.DeleteAsync(id);
            return NoContent();
        }

    }
}