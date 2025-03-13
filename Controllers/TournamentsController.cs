using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace tenis_pro_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TournamentsController : ControllerBase
	{
		private readonly TournamentsRepository _tournamentRepository;

		public TournamentsController(TournamentsRepository tournamentRepository)
		{
			_tournamentRepository = tournamentRepository;
		}

		// GET: api/tournaments
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Tournament>>> GetAll()
		{
			var tournaments = await _tournamentRepository.GetAllTournamentsAsync();
			return Ok(tournaments);
		}

		// GET: api/tournaments/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Tournament>> GetById(string id)
		{
			var tournament = await _tournamentRepository.GetTournamentByIdAsync(id);

			if (tournament == null)
			{
				return NotFound();
			}

			return Ok(tournament);
		}

		// POST: api/tournaments
		[HttpPost]
		public async Task<ActionResult> Create(Tournament tournament)
		{
			tournament.Id = null; // Deja que MongoDB genere el Id automáticamente
			await _tournamentRepository.CreateTournamentAsync(tournament);
			return CreatedAtAction(nameof(GetById), new { id = tournament.Id }, tournament);
		}

		// PUT: api/tournaments/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, Tournament tournament)
		{
			var existingTournament = await _tournamentRepository.GetTournamentByIdAsync(id);

			if (existingTournament == null)
			{
				return NotFound();
			}

			await _tournamentRepository.UpdateTournamentAsync(id, tournament);
			return NoContent();
		}

		// DELETE: api/tournaments/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var tournament = await _tournamentRepository.GetTournamentByIdAsync(id);

			if (tournament == null)
			{
				return NotFound();
			}

			await _tournamentRepository.DeleteTournamentAsync(id);
			return NoContent();

		}
	}
}
