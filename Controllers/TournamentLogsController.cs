using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentLogsController : ControllerBase
    {
        private readonly ITournamentLog _tournamentLogRepository;

        public TournamentLogsController(ITournamentLog tournamentLogRepository)
        {
            _tournamentLogRepository = tournamentLogRepository;
        }

        [HttpGet("{tournamentId}/logs")]
        public IActionResult GetLogs(string tournamentId)
        {
            var logs = _tournamentLogRepository.GetLogsByTournament(tournamentId);
            return Ok(logs);
        }
    }
}
