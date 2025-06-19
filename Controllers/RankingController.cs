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

        public RankingController(IMongoDatabase database)
        {
            _tournamentsCollection = database.GetCollection<Tournament>("Tournaments"); 
            _matchesCollection = database.GetCollection<Match>("Matches");
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
                if (match.Result == null || match.registrations == null || match.registrations.Count != 2)
                    continue;

                var winnerId = match.Result.Winner;
                var loserId = match.registrations.First(r => r != winnerId);
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
    }
}