using Microsoft.AspNetCore.Mvc;
using static tenis_pro_back.Models.Match;
using tenis_pro_back.Repositories;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Controllers
{
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
            var existing = await _repository.GetById(id);
            if (existing == null) return NotFound();

            await _repository.Delete(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddMatch([FromBody] Match match)
        {
            await _repository.Post(match);
            return Ok();
        }

        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduledMatches()
        {
            var matches = await _repository.GetScheduledMatchesAsync();
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchById(string id)
        {
            var match = await _repository.GetById(id);
            if (match == null) return NotFound();
            return Ok(match);
        }

        [HttpGet]
        public async Task<IActionResult> GetMatches()
        {
            var match = await _repository.GetAll();
            if (match == null) return NotFound();
            return Ok(match);
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetMatchHistory(string id)
        {
            var history = await _repository.GetMatchHistoryAsync(id);
            return Ok(history);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] MatchStatus status, [FromBody] string? notes)
        {
            await _repository.UpdateMatchStatusAsync(id, status, notes);
            return Ok();
        }

        [HttpPut("{id}/result")]
        public async Task<IActionResult> AddResult(string id, [FromBody] string result)
        {
            await _repository.AddMatchResultAsync(id, result);
            return Ok();
        }
    }
}
