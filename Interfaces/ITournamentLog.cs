using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface ITournamentLog
    {
        Task AddLog(TournamentLog log);
        Task<List<TournamentLog>> GetLogsByTournament(string tournamentId);
    }
}
