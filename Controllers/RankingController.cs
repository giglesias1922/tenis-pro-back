using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController:Controller
    {
        private readonly IMongoCollection<Tournament> _tournamentsCollection;
        private readonly IMongoCollection<Models.Match> _matchesCollection;
        private readonly IMongoCollection<PlayerRankingStats> _playerRankingStatsCollection;

        public RankingController(IMongoDatabase database)
        {
            _tournamentsCollection = database.GetCollection<Tournament>("Tournaments"); 
            _matchesCollection = database.GetCollection<Match>("Matches");
            _playerRankingStatsCollection = database.GetCollection<PlayerRankingStats>("PlayerRankingStats");
        }

        


        [HttpPost]
        public async Task<IActionResult> GetRanking([FromBody] RankingRequestDto request)
        {
            var selectedYear = request.Year;

            var tournaments = await _tournamentsCollection
                .Find(t => t.CategoryId == request.CategoryId
                        && t.TournamentType == request.TournamentType
                        && t.Status== Models.Enums.TournamentStatusEnum.Finalized
                        && t.CloseDate.Value.Year == selectedYear)
                .ToListAsync();

            var tournamentIds = tournaments.Select(t => t.Id).ToList();

            var matches = await _matchesCollection
                .Find(m => tournamentIds.Contains(m.TournamentId) && m.Status == Match.MatchStatus.Completed)
                .ToListAsync();

            var playerStats = new Dictionary<string, PlayerRankingStats>();

            foreach (var match in matches)
            {
                if (match.Result == null || match.Participant1Id == null || match.Participant2Id == null)
                    continue;

                var winnerId = match.Result.Winner;
                var loserId = match.Participant1Id == winnerId ? match.Participant2Id : match.Participant1Id;
                var points = match.Result.Points ?? 0;

                // Winner
                if (!playerStats.ContainsKey(winnerId))
                    playerStats[winnerId] = new PlayerRankingStats(winnerId);

                playerStats[winnerId].MatchesPlayed++;
                playerStats[winnerId].MatchesWon++;
                playerStats[winnerId].TotalPoints += points;

                // Loser
                if (!playerStats.ContainsKey(loserId))
                    playerStats[loserId] = new PlayerRankingStats(loserId);

                playerStats[loserId].MatchesPlayed++;
            }

            var ranking = playerStats.Values
                .OrderByDescending(p => p.TotalPoints)
                .ThenByDescending(p => p.MatchesWon)
                .ToList();

            return Ok(ranking);
        }

        [HttpPost("init")]
        public async Task<IActionResult> InitRanking([FromBody] List<PlayerRankingStats> initialRankings)
        {
            if (initialRankings == null || !initialRankings.Any())
                return BadRequest("No ranking data provided.");

            await _playerRankingStatsCollection.InsertManyAsync(initialRankings);
            return Ok(new { message = "Initial rankings loaded successfully." });
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRanking([FromBody] PlayerRankingStats ranking)
        {
            if (ranking == null || string.IsNullOrEmpty(ranking.PlayerId))
                return BadRequest("Invalid ranking data.");

            await _playerRankingStatsCollection.InsertOneAsync(ranking);
            return Ok(new { message = "Ranking entry added successfully." });
        }
    }
}