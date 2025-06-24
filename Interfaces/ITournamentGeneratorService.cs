using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface ITournamentGeneratorService
    {
        Task<Tournament> GenerateDraw(string tournamentId);
    }
} 