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
    public class RegistrationsController : ControllerBase
    {
        private readonly ITournament _tournamentRepository;
        private readonly IUser _userRepository;

        public RegistrationsController(ITournament tournamentRepository, IUser userRepository)
        {
            _tournamentRepository = tournamentRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("tournament/{tournamentId}")]
        public async Task<ActionResult<IEnumerable<RegistrationsDto>>> GetRegistrations(string tournamentId)
        {
            try
            {
                var tournament = await _tournamentRepository.GetById(tournamentId);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                var registrations = tournament.Participants.Select(p => new RegistrationsDto
                {
                    Id = p.Id,
                    TournamentId = tournamentId,
                    DisplayName = p.DisplayName,
                    TournamentDescription = tournament.Description,
                    CreatedAt = DateTime.UtcNow // TODO: Agregar campo CreatedAt al modelo Participant
                });

                return Ok(registrations);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("users/{tournamentId}")]
        public async Task<ActionResult<IEnumerable<RegistrationUserDto>>> GetRegistratedUsers(string tournamentId)
        {
            try
            {
                var tournament = await _tournamentRepository.GetById(tournamentId);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                var registeredUsers = tournament.Participants.Select(p => new RegistrationUserDto
                {
                    Id = p.Id,
                    DisplayName = p.DisplayName
                });

                return Ok(registeredUsers);
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
                var tournament = await _tournamentRepository.GetById(tournamentId);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                // Obtener todos los usuarios de la categoría
                var allUsers = await _userRepository.GetByCategory(categoryId);
                
                // Filtrar usuarios que ya están registrados
                var registeredPlayerIds = tournament.Participants
                    .SelectMany(p => p.PlayerIds)
                    .Distinct()
                    .ToList();

                var availableUsers = allUsers.Where(u => !registeredPlayerIds.Contains(u.Id)).ToList();

                return Ok(availableUsers);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateRegistration([FromBody] CreateRegistrationDto registrationDto)
        {
            try
            {
                var tournament = await _tournamentRepository.GetById(registrationDto.TournamentId);
                if (tournament == null)
                {
                    return NotFound("Torneo no encontrado");
                }

                // Verificar que el torneo esté abierto para inscripciones
                if (tournament.Status != Models.Enums.TournamentStatusEnum.Pending)
                {
                    return BadRequest("El torneo no está abierto para inscripciones");
                }

                // Verificar que los jugadores no estén ya registrados
                var registeredPlayerIds = tournament.Participants
                    .SelectMany(p => p.PlayerIds)
                    .Distinct()
                    .ToList();

                var duplicatePlayers = registrationDto.Players.Where(p => registeredPlayerIds.Contains(p)).ToList();
                if (duplicatePlayers.Any())
                {
                    return BadRequest("Uno o más jugadores ya están registrados en este torneo");
                }

                // Crear el participante
                var participant = new Participant
                {
                    PlayerIds = registrationDto.Players,
                    DisplayName = registrationDto.DisplayName,
                    Ranking = 999 // Se actualizará cuando se genere el draw
                };

                tournament.Participants.Add(participant);
                await _tournamentRepository.Put(tournament.Id, tournament);

                return CreatedAtAction(nameof(GetRegistrations), new { tournamentId = tournament.Id }, participant);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{registrationId}")]
        public async Task<ActionResult> DeleteRegistration(string registrationId)
        {
            try
            {
                // Buscar el torneo que contiene esta inscripción
                var allTournaments = await _tournamentRepository.GetAll();
                var tournamentDto = allTournaments.FirstOrDefault(t => t.Participants.Any(p => p.Id == registrationId));

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
                if (tournament.Status != Models.Enums.TournamentStatusEnum.Pending)
                {
                    return BadRequest("No se puede eliminar inscripciones de un torneo que ya comenzó");
                }

                // Remover el participante
                tournament.Participants.RemoveAll(p => p.Id == registrationId);
                await _tournamentRepository.Put(tournament.Id, tournament);

                return NoContent();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateRegistrationDto
    {
        public required string TournamentId { get; set; }
        public required List<string> Players { get; set; }
        public required string DisplayName { get; set; }
        public string? CreatedBy { get; set; }
    }
} 