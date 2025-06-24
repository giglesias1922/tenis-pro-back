using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Interfaces
{
    public interface ITournament
    {
        Task<IEnumerable<TournamentDetailDto>> GetAll();
        Task<TournamentDetailDto?> GetById(string id);
        Task Post(Tournament tournament);
        Task Put(string id, Tournament tournament);
        Task Delete(string id);
        Task<IEnumerable<TournamentDetailDto>> GetTournamentsWithOpenRegistrations();
        Task<IEnumerable<TournamentDetailDto>> GetTournamentsToProgramming();
        Task<IEnumerable<TournamentDetailDto>> GetTournamentsActives();
        Task<IEnumerable<TournamentBoardDto>> GetTournamentsBoard();
    }
}
