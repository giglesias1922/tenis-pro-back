using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace tenis_pro_back.Controllers
{
    [Authorize]
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
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// GET: api/location/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Location>> GetById(string id)
		{
			try
			{
				var location = await _locationRepository.GetById(id);
				if (location == null)
				{
					return NotFound();
				}
				return Ok(location);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// POST: api/location
		[HttpPost]
		public async Task<ActionResult<Location>> Create(Location location) 
		{
			try
			{
				location.Id = null; // MongoDB generates the ID
				await _locationRepository.Post(location);
				return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// PUT: api/location/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, Location location)
		{
			try
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
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// DELETE: api/location/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			try
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
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
	}
}
