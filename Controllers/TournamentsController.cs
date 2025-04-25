using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class TournamentsController : ControllerBase
	{
		private readonly ITournament _tournamentRepository;

		public TournamentsController(ITournament tournamentRepository)
		{
			_tournamentRepository = tournamentRepository;
		}

        [HttpGet("open-registrations")]
        public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetTournamentsWithOpenRegistrations()
        {
            var tournaments = await _tournamentRepository.GetTournamentsWithOpenRegistrations();
            return Ok(tournaments);
        }


        [HttpGet("to-programming")]
        public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetTournamentsToProgramming()
        {
            var tournaments = await _tournamentRepository.GetTournamentsToProgramming();
            return Ok(tournaments);
        }
        

        // GET: api/tournaments
        [HttpGet]
		public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetAll()
		{
			var tournaments = await _tournamentRepository.GetAll();
			return Ok(tournaments);
		}

		// GET: api/tournaments/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<TournamentDetailDto>> GetById(string id)
		{
			var tournament = await _tournamentRepository.GetById(id);

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
			await _tournamentRepository.Post(tournament);
			return CreatedAtAction(nameof(GetById), new { id = tournament.Id }, tournament);
		}

		// PUT: api/tournaments/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, Tournament tournament)
		{
			var existingTournament = await _tournamentRepository.GetById(id);

			if (existingTournament == null)
			{
				return NotFound();
			}

			await _tournamentRepository.Put(id, tournament);
			return NoContent();
		}

		// DELETE: api/tournaments/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var tournament = await _tournamentRepository.GetById(id);

			if (tournament == null)
			{
				return NotFound();
			}

			await _tournamentRepository.Delete(id);
			return NoContent();

		}
	}
}
