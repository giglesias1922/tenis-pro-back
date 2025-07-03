using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Helpers;
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
        private readonly ITournamentGeneratorService _tournamentGeneratorService;

        public TournamentsController(ITournament tournamentRepository, ITournamentGeneratorService tournamentGeneratorService)
		{
			_tournamentRepository = tournamentRepository;
            _tournamentGeneratorService = tournamentGeneratorService;
        }

        [Authorize]
        [HttpGet("open-registrations")]
        public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetTournamentsWithOpenRegistrations()
        {
			try
			{
				var tournaments = await _tournamentRepository.GetTournamentsWithOpenRegistrations();
				return Ok(tournaments);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpGet("tournament-types")]
        public IActionResult GetTournamentTypes()
        {
            var enumList = Enum.GetValues(typeof(Models.Enums.TournamentTypeEnum))
                .Cast<Models.Enums.TournamentTypeEnum>()
                .Select(e => new {
                    id = (int)e,
                    description = e.ToString()
                });

            return Ok(enumList);
        }

        [AllowAnonymous]
        [HttpGet("tournament-status")]
        public IActionResult GetTournamentStatus()
        {
            var enumList = Enum.GetValues(typeof(Models.Enums.TournamentStatusEnum))
                .Cast<Models.Enums.TournamentStatusEnum>()
                .Select(e => new {
                    id = (int)e,
                    description = e.ToString()
                });

            return Ok(enumList);
        }

        [Authorize]
        [HttpGet("to-programming")]
        public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetTournamentsToProgramming()
        {
			try
			{
				var tournaments = await _tournamentRepository.GetTournamentsToProgramming();
				return Ok(tournaments);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }


        // GET: api/tournaments
        [HttpGet]
		public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetAll()
		{
			try
			{
				var tournaments = await _tournamentRepository.GetAll();
				return Ok(tournaments);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// GET: api/tournaments/{id}
		[HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TournamentDetailDto>> GetById(string id)
		{
			try
			{
				var tournament = await _tournamentRepository.GetDtoById(id);

				if (tournament == null)
				{
					return NotFound();
				}

				return Ok(tournament);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// POST: api/tournaments
		[HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(Tournament tournament)
		{
			try
			{
				tournament.Id = null; // Deja que MongoDB genere el Id automáticamente
				await _tournamentRepository.Post(tournament);
				return CreatedAtAction(nameof(GetById), new { id = tournament.Id }, tournament);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// PUT: api/tournaments/{id}
		[HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, Tournament tournament)
		{
			try
			{
				var existingTournament = await _tournamentRepository.GetDtoById(id);

				if (existingTournament == null)
				{
					return NotFound();
				}

				await _tournamentRepository.Put(id, tournament);
				return NoContent();
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

		// DELETE: api/tournaments/{id}
		[HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
		{
			try
			{
				var tournament = await _tournamentRepository.GetDtoById(id);

				if (tournament == null)
				{
					return NotFound();
				}

				await _tournamentRepository.Delete(id);
				return NoContent();
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }

        }

        [AllowAnonymous]
        [HttpGet("board")]
        public async Task<ActionResult<IEnumerable<TournamentBoardDto>>> GetTournamentsBoard()
        {
            try
            {
                IEnumerable<TournamentBoardDto> tournaments = await _tournamentRepository.GetTournamentsBoard();
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [Authorize]
        [HttpPost("{id}/close-registrations")]
        public async Task<IActionResult> CloseRegistrations(string id)
        {
            try
            {
                var tournament = await _tournamentRepository.GetById(id);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                // Verificar que el torneo esté en estado Pending
                if (tournament.Status != Models.Enums.TournamentStatusEnum.Pending)
                {
                    return BadRequest("Solo se pueden cerrar inscripciones de torneos en estado Pending");
                }

                // Verificar que tenga participantes
                if (!tournament.Participants.Any())
                {
                    return BadRequest("No se puede cerrar inscripciones de un torneo sin participantes");
                }

                // Cambiar el estado a Programming
                tournament.Status = Models.Enums.TournamentStatusEnum.Programming;
                
                await _tournamentRepository.Put(id, tournament);
                
                return Ok(new { message = "Inscripciones cerradas exitosamente" });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{id}/generate-draw")]
        public async Task<IActionResult> GenerateDraw(string id, [FromBody] DrawConfigurationDto config)
        {
            try
            {
                // Obtener el torneo
                var tournament = await _tournamentRepository.GetById(id);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                // Verificar que el torneo esté en estado Programming
                if (tournament.Status != Models.Enums.TournamentStatusEnum.Programming)
                {
                    return BadRequest("Solo se puede generar el draw de torneos en estado Programming");
                }

                // Verificar que tenga participantes
                if (!tournament.Participants.Any())
                {
                    return BadRequest("No se puede generar el draw de un torneo sin participantes");
                }

                // Actualizar la configuración del torneo
                tournament.IncludePlata = config.IncludePlata;
                tournament.PlayersPerZone = config.PlayersPerZone;
                tournament.QualifiersPerZone = config.QualifiersPerZone;

                // Generar el draw
                var result = await _tournamentGeneratorService.GenerateDraw(id, config);
                
                return Ok(new { message = "Draw generado exitosamente", tournament = result });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
