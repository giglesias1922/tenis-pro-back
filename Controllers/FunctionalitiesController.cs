using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FunctionalitiesController : ControllerBase
    {
        private readonly IFunctionality _functionalitiesRepository;

        public FunctionalitiesController(IFunctionality functionalitiesRepository)
        {
            _functionalitiesRepository = functionalitiesRepository;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<List<Functionality>>> Get()
        {
            var list = await _functionalitiesRepository.GetAll();
            return Ok(list.OrderBy(o => o.Name));
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Functionality>> Get(string id)
        {
            var obj = await _functionalitiesRepository.GetById(id);
            if (obj == null) return NotFound();
            return Ok(obj);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult> Create(Functionality obj)
        {
            await _functionalitiesRepository.Post(obj);
            return CreatedAtAction(nameof(Get), new { id = obj.Id }, obj);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Functionality obj)
        {
            var existingObj = await _functionalitiesRepository.GetById(id);
            if (existingObj == null) return NotFound();

            await _functionalitiesRepository.Put(id, obj);
            return NoContent();
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingObj = await _functionalitiesRepository.GetById(id);
            if (existingObj == null) return NotFound();


            await _functionalitiesRepository.Delete(id);
            return NoContent();
        }

    }
}