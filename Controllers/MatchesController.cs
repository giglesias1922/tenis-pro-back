using Microsoft.AspNetCore.Mvc;
using static tenis_pro_back.Models.Match;
using tenis_pro_back.Repositories;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace tenis_pro_back.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class MatchesController : ControllerBase
    {
        private readonly IMatch _repository;

        public MatchesController(IMatch repository)
        {
            _repository = repository;
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var existing = await _repository.GetById(id);
                if (existing == null) return NotFound();

                await _repository.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        {
            try
            {
                var match = new Match
                {
                    TournamentId = dto.TournamentId,
                    Participant1Id = dto.Participant1Id,
                    Participant2Id = dto.Participant2Id,
                    Status = Match.MatchStatus.Scheduled,
                    CreatedAt = DateTime.UtcNow,
                    History = new List<Match.MatchHistory>
                {
                    new Match.MatchHistory
                    {
                        Date = DateTime.UtcNow,
                        Status = Match.MatchStatus.Scheduled,
                        Notes = $"Partido programado para el {dto.ScheduledDate:dd/MM/yyyy HH:mm}"
                    }
                }
                };

                await _repository.Post(match);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduledMatches()
        {
            try
            {
                var matches = await _repository.GetScheduledMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchById(string id)
        {
            try
            {
                var match = await _repository.GetById(id);
                if (match == null) return NotFound();
                return Ok(match);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMatches()
        {
            try
            {
                var match = await _repository.GetAll();
                if (match == null) return NotFound();
                return Ok(match);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetMatchHistory(string id)
        {
            try
            {
                var history = await _repository.GetMatchHistoryAsync(id);
                return Ok(history);
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] MatchStatus status, [FromBody] string? notes)
        {
            try
            {
                await _repository.UpdateMatchStatusAsync(id, status, notes);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPut("{id}/result")]
        public async Task<IActionResult> AddResult(string id, [FromBody] MatchResultDto result)
        {
            try
            {
                await _repository.AddMatchResultAsync(id, result);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }
    }
}
