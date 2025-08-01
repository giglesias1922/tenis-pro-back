using MongoDB.Driver;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Interfaces
{
    public interface ITournamentProgressService
    {
        string GetNextRoundName(string currentRound);
        Task RegisterMatchResult(string id, MatchResultDto result);
    }
}
