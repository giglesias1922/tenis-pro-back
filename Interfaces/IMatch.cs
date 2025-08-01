using MongoDB.Driver;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Interfaces
{
    public interface IMatch
    {
        Task Post(Match match);
        Task<List<Match>> GetScheduledMatchesAsync();
        Task<MatchDto?> GetById(string id);
        Task<List<MatchHistory>> GetMatchHistoryAsync(string id);
        Task UpdateMatchStatusAsync(string id, MatchStatus status, string? notes);

        Task Delete(string id);
        Task<List<MatchDto>> GetAll();

        Task<Match> CreateMatch(Match match, IClientSessionHandle session);
        Task<List<Match>> GetMatchesByZone(string tournamentId, string zoneId);

        Task<List<Match>> GetMatchesByTournament(string tournamentId, IClientSessionHandle? session = null);

        Task<List<Match>> GetMatchesByIdsAsync(List<string> matchIds);

        Task AddMatchResult(string id, MatchResultDto dto, IClientSessionHandle session);
    }
}
