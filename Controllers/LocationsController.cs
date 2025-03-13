using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace tenis_pro_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LocationsController : ControllerBase
	{
		private readonly LocationsRepository _locationRepository;
		private readonly TournamentsRepository _tournamentRepository;


        public LocationsController(LocationsRepository locationRepository, TournamentsRepository tournamentRepository)
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
				var locations = await _locationRepository.GetLocationsAsync();
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
			var location = await _locationRepository.GetLocationByIdAsync(id);
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
			await _locationRepository.CreateLocationAsync(location);
			return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
		}

		// PUT: api/location/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, Location location)
		{
			var existingLocation = await _locationRepository.GetLocationByIdAsync(id);
			if (existingLocation == null)
			{
				return NotFound();
			}

			location.Id = id; // Ensure the ID remains the same
			var updated = await _locationRepository.UpdateLocationAsync(id, location);

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
			var location = await _locationRepository.GetLocationByIdAsync(id);
			if (location == null)
			{
				return NotFound();
			}

            // Verificar si hay torneos asociados a esta categoría
            long tournamentCount = await _tournamentRepository.CountTournamentsByLocationIdAsync(id);
            if (tournamentCount > 0)
            {
                return BadRequest("No se puede eliminar la sede porque hay torneos asociados a ella.");
            }

            var deleted = await _locationRepository.DeleteLocationAsync(id);
			if (!deleted)
			{
				return StatusCode(500, "Error deleting the location");
			}

			return NoContent();
		}
	}
}
