using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Helpers;

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
            try
            {
                var list = await _functionalitiesRepository.GetAll();
                return Ok(list.OrderBy(o => o.Name));
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Functionality>> Get(string id)
        {
            try
            {
                var obj = await _functionalitiesRepository.GetById(id);
                if (obj == null) return NotFound();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult> Create(Functionality obj)
        {
            try
            {
                await _functionalitiesRepository.Post(obj);
                return CreatedAtAction(nameof(Get), new { id = obj.Id }, obj);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Functionality obj)
        {
            try
            {
                var existingObj = await _functionalitiesRepository.GetById(id);
                if (existingObj == null) return NotFound();

                await _functionalitiesRepository.Put(id, obj);
                return NoContent();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var existingObj = await _functionalitiesRepository.GetById(id);
                if (existingObj == null) return NotFound();


                await _functionalitiesRepository.Delete(id);
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