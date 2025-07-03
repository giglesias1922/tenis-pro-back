using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Interfaces
{
    public interface ITournamentGeneratorService
    {
        Task<Tournament> GenerateDraw(string tournamentId, DrawConfigurationDto config);
    }
} 