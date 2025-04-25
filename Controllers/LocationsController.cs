using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LocationsController : ControllerBase
	{
		private readonly ILocation _locationRepository;
		private readonly ITournament _tournamentRepository;


        public LocationsController(ILocation locationRepository, ITournament tournamentRepository)
        {
            _locationRepository = locationRepository;
            _tournamentRepository = tournamentRepository;
        }

        // GET: api/location
        [HttpGet]
		public async Task<ActionResult<IEnumerable<Location>>> GetAll()
		{
			try
			{
				var locations = await _locationRepository.GetAll();
				return Ok(locations);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: api/location/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Location>> GetById(string id)
		{
			var location = await _locationRepository.GetById(id);
			if (location == null)
			{
				return NotFound();
			}
			return Ok(location);
		}

		// POST: api/location
		[HttpPost]
		public async Task<ActionResult<Location>> Create(Location location) 
		{
			location.Id = null; // MongoDB generates the ID
			await _locationRepository.Post(location);
			return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
		}

		// PUT: api/location/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, Location location)
		{
			var existingLocation = await _locationRepository.GetById(id);
			if (existingLocation == null)
			{
				return NotFound();
			}

			location.Id = id; // Ensure the ID remains the same
			var updated = await _locationRepository.Put(id, location);

			if (!updated)
			{
				return StatusCode(500, "Error updating the location");
			}

			return NoContent();
		}

		// DELETE: api/location/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var location = await _locationRepository.GetById(id);
			if (location == null)
			{
				return NotFound();
			}

            var deleted = await _locationRepository.Delete(id);

			if (!deleted)
			{
				return StatusCode(500, "Error deleting the location");
			}

			return NoContent();
		}
	}
}
