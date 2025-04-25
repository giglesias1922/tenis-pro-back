using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Interfaces
{
    public interface IMatch
    {
        Task Post(Match match);
        Task<List<Match>> GetScheduledMatchesAsync();
        Task<Match?> GetById(string id);
        Task<List<MatchHistory>> GetMatchHistoryAsync(string id);
        Task UpdateMatchStatusAsync(string id, MatchStatus status, string? notes);
        Task AddMatchResultAsync(string id, string result);

        Task Delete(string id);
        Task<List<MatchDto>> GetAll();
    }
}
