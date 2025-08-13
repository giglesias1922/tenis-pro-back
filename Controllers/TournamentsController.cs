using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Models.Response;
using tenis_pro_back.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tenis_pro_back.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class TournamentsController : ControllerBase
	{
        private readonly ITournament _tournamentRepository;
        private readonly ITournamentGeneratorService _tournamentGeneratorService;
        private readonly IUser _userRepository;

        public TournamentsController(ITournament tournamentRepository, ITournamentGeneratorService tournamentGeneratorService, IUser usersRepository)
        {
            _tournamentRepository = tournamentRepository;
            _tournamentGeneratorService = tournamentGeneratorService;
            _userRepository = usersRepository;
        }

        [Authorize]
        [HttpGet("open-registrations")]
        public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetTournamentsWithOpenRegistrations()
        {
			try
			{
				var tournaments = await _tournamentRepository.GetTournamentsWithOpenRegistrations();

                return Ok(new ApiResponse<IEnumerable<TournamentDetailDto>>(tournaments, "Torneos obtenidos"));
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

            return Ok(new ApiResponse<object>(enumList, "Tipos de Torneos obtenidos"));

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

            return Ok(new ApiResponse<object>(enumList, "Estados de Torneos obtenidos"));
        }

        [Authorize]
        [HttpGet("to-programming")]
        public async Task<ActionResult<IEnumerable<TournamentDetailDto>>> GetTournamentsToProgramming()
        {
			try
			{
				var tournaments = await _tournamentRepository.GetTournamentsToProgramming();

                return Ok(new ApiResponse<IEnumerable<TournamentDetailDto>>(tournaments, "Torneos obtenidos"));
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
                return Ok(new ApiResponse<IEnumerable<TournamentDetailDto>>(tournaments, "Torneos obtenidos"));
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // GET: api/tournaments/{id}
        [HttpGet("{id}/zone-draw")]
        public async Task<ActionResult<List<TournamentZone>>> GetZonesDraw(string id)
        {
            try
            {
                var tournament = await _tournamentRepository.GetDtoById(id);

                if (tournament == null)
                {
                    return NotFound(new ApiResponse<TournamentDetailDto>(
                                    null,
                                    "Torneo no encontrado",
                                    success: false
                                ));
                }

                List<TournamentZone> zones = await _tournamentGeneratorService.GetZonesDraw(id);

                return Ok(new ApiResponse<List<TournamentZone>>(zones, "Registros obtenidos"));
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
					return NotFound(new ApiResponse<TournamentDetailDto>(
                                    null,
                                    "Torneo no encontrado",
                                    success: false
                                ));
                }

                return Ok(new ApiResponse<TournamentDetailDto>(tournament, "Torneo obtenidos"));
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
                tournament.Status = Models.Enums.TournamentStatusEnum.OpenRegistration;
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
		[HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] TournamentUpdateDto tournament)
		{
			try
			{
				var existingTournament = await _tournamentRepository.GetDtoById(id);

				if (existingTournament == null)
				{
					return NotFound();
				}

				await _tournamentRepository.UpdateTournament(id, tournament);
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

                return Ok(new ApiResponse<TournamentDetailDto>(tournament, "Torneo eliminado"));
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
                return Ok(new ApiResponse<IEnumerable<TournamentBoardDto>>(tournaments, "Torneos board cargado"));
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
                if (tournament.Status != Models.Enums.TournamentStatusEnum.OpenRegistration)
                {
                    return BadRequest("Solo se pueden cerrar inscripciones de torneos en estado Pending");
                }

                // Verificar que tenga participantes
                if (!tournament.Participants.Any())
                {
                    return BadRequest("No se puede cerrar inscripciones de un torneo sin participantes");
                }

                // Cambiar el estado a Programming
                await _tournamentRepository.UpdateStatus(id, Models.Enums.TournamentStatusEnum.CloseRegistration);
                
                return Ok(new { message = "Inscripciones cerradas exitosamente" });
                new ApiResponse<object>(null, "Inscripciones cerradas exitosamente");
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
                if (tournament.Status != Models.Enums.TournamentStatusEnum.CloseRegistration)
                {
                    return BadRequest("Solo se puede generar el draw de torneos que hayan cerrado la inscripción");
                }

                // Verificar que tenga participantes
                if (!tournament.Participants.Any())
                {
                    return BadRequest("No se puede generar el draw de un torneo sin participantes");
                }

                // Generar el draw
                Tournament result = await _tournamentGeneratorService.GenerateDraw(id, config);

                return Ok(new ApiResponse<Tournament>(result, "Draw generado exitosamente"));
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/participants/add")]
        [Authorize]
        public async Task<IActionResult> AddParticipant(string id, [FromBody] Participant participant)
        {
            try
            {
                var tournament = await _tournamentRepository.GetById(id);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                // Verificar que el torneo esté abierto para inscripciones
                if (tournament.Status != Models.Enums.TournamentStatusEnum.OpenRegistration)
                {
                    return Ok(new ApiResponse<Tournament>(null, "El torneo no está abierto para inscripciones",false));

                }

                // Verificar que los jugadores no estén ya registrados
                var registeredPlayerIds = tournament.Participants
                    .SelectMany(p => p.PlayerIds)
                    .Distinct()
                    .ToList();

                var duplicatePlayers = participant.PlayerIds.Where(p => registeredPlayerIds.Contains(p)).ToList();
                if (duplicatePlayers.Any())
                {
                    return Ok(new ApiResponse<Tournament>(null, "Ya esta registrado en este torneo.", false));
                }

                await _tournamentRepository.AddParticipant(id, participant);

                return Ok(new ApiResponse<object>(null, "Participante agregado."));

            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{tournamentId}/participants/{participantId}")]
        [Authorize]
        public async Task<IActionResult> RemoveParticipant(string tournamentId, string participantId)
        {
            try
            {
                // Buscar el torneo que contiene esta inscripción
                var allTournaments = await _tournamentRepository.GetAll();
                var tournamentDto = allTournaments.FirstOrDefault(t => t.Participants.Any(p => p.Id == participantId));

                if (tournamentDto == null)
                {
                    return NotFound("Inscripción no encontrada");
                }

                // Obtener el torneo completo
                var tournament = await _tournamentRepository.GetById(tournamentDto.Id);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                // Verificar que el torneo esté abierto para inscripciones
                if (tournament.Status != Models.Enums.TournamentStatusEnum.OpenRegistration)
                {
                    return BadRequest("No se puede eliminar inscripciones de un torneo que ya comenzó");
                }


                var result = await _tournamentRepository.RemoveParticipant(tournamentId, participantId);

                if (!result)
                {
                    return NotFound("Torneo o participante no encontrado.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: api/tournaments/{tournamentId}/participants
        [HttpGet("{tournamentId}/participants")]
        public async Task<IActionResult> GetParticipants(string tournamentId)
        {
            try
            {
                var participants = await _tournamentRepository.GetParticipants(tournamentId);
                if (participants == null)
                    return NotFound("Torneo no encontrado");

                return Ok(new ApiResponse<List<Participant>>(participants, "Participantes obtenidos."));
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{categoryId}/{tournamentId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersToRegistration(string categoryId, string tournamentId)
        {
            try
            {
                var users = await _tournamentRepository.GetParticipantsToRegister(categoryId, tournamentId);

                return Ok(new ApiResponse<List<User>>(users, "Usuarios obtenidos."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }


        // GET: api/tournaments/{tournamentId}/participants/{participantId}
        [HttpGet("{tournamentId}/participants/{participantId}")]
        public async Task<IActionResult> GetParticipant(string tournamentId, string participantId)
        {
            try
            {
                var participant = await _tournamentRepository.GetParticipant(tournamentId, participantId);
                if (participant == null)
                    return NotFound("Participante no encontrado");

                return Ok(new ApiResponse<Participant>(participant, "Participante obtenido."));
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        //Este metodo es para pruebas, para agregar incripciones masivas
        [HttpPost("register-batch/{tournamentId}/{cantidad}")]
        public async Task<IActionResult> RegisterBatchParticipants(string tournamentId, int cantidad)
        {
            try
            {
                // Obtener torneo
                var tournament = await _tournamentRepository.GetById(tournamentId);
                if (tournament == null)
                    return NotFound("Torneo no encontrado.");

                var categoryId = tournament.CategoryId;
                if (string.IsNullOrEmpty(categoryId))
                    return BadRequest("El torneo no tiene categoría definida.");

                // Obtener usuarios habilitados de esa categoría
                var allUsers = await _userRepository.GetByCategory(categoryId);
                var enabledUsers = allUsers.Where(u => u.Status == UserStatus.Enabled).ToList();

                if (!enabledUsers.Any())
                    return NotFound("No hay jugadores habilitados en la categoría.");

                // IDs de jugadores ya registrados en el torneo
                var registeredPlayerIds = tournament.Participants.SelectMany(p => p.PlayerIds).ToHashSet();

                // Filtrar solo los usuarios no registrados y limitar por cantidad
                var newPlayers = enabledUsers.Where(u => !registeredPlayerIds.Contains(u.Id)).Take(cantidad).ToList();

                if (!newPlayers.Any())
                    return BadRequest("No hay jugadores nuevos disponibles para registrar.");

                // Registrar uno a uno usando AddParticipant
                foreach (var user in newPlayers)
                {
                    var participant = new Participant
                    {
                        PlayerIds = new List<string> { user.Id! },
                        DisplayName = $"{user.Name} {user.LastName}"
                    };

                    await _tournamentRepository.AddParticipant(tournamentId, participant);
                }

                return Ok(new
                {
                    message = $"{newPlayers.Count} jugadores registrados correctamente.",
                    jugadores = newPlayers.Select(u => new { u.Id, u.Name, u.LastName })
                });
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return StatusCode(500, "Error al registrar jugadores.");
            }
        }


        
        [HttpPost("ResetTournamentDraw/{tournamentId}")]
        public async Task<IActionResult> ResetTournamentDraw(string tournamentId)
        {
            if (string.IsNullOrEmpty(tournamentId))
                return BadRequest("Tournament ID is required.");

            try
            {
                await _tournamentRepository.ResetTournamentDraw(tournamentId);

                return Ok("Tournament status updated and zones cleared.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/draw")]
        public async Task<IActionResult> GetTournamentDraw(string id)
        {
            var tournament = await _tournamentRepository.GetById(id);
            if (tournament == null)
                return NotFound();

            return Ok(new
            {
                mainBracket = tournament.MainBracket,
                silverCupBracket = tournament.SilverCupBracket
            });
        }
    }
}
