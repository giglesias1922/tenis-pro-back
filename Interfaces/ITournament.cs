using MongoDB.Driver;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Models.Enums;

namespace tenis_pro_back.Interfaces
{
    public interface ITournament
    {
        Task<IEnumerable<TournamentDetailDto>> GetAll();
        Task<TournamentDetailDto?> GetDtoById(string id);
        Task Post(Tournament tournament);
        Task UpdateTournament(string id, TournamentUpdateDto tournament);
        Task Delete(string id);
        Task<IEnumerable<TournamentDetailDto>> GetTournamentsWithOpenRegistrations();
        Task<IEnumerable<TournamentDetailDto>> GetTournamentsToProgramming();
        Task<IEnumerable<TournamentDetailDto>> GetTournamentsActives();
        Task<IEnumerable<TournamentBoardDto>> GetTournamentsBoard();
        Task<Tournament?> GetById(string id);
        Task AddParticipant(string id, Participant participant);
        Task<bool> RemoveParticipant(string tournamentId, string participantId);
        Task<Participant> GetParticipant(string tournamentId, string participantId);
        Task<List<Participant>> GetParticipants(string tournamentId);
        Task<List<User>> GetParticipantsToRegister(string categoryId, string tournamentId);
        Task UpdateStatus(string tournamentId, TournamentStatusEnum newStatus);
        Task UpdateStatus(string tournamentId, TournamentStatusEnum newStatus, IClientSessionHandle session);
        Task UpdateDraw(TournamentUpdateDrawDto draw, IClientSessionHandle session);
        Task ResetTournamentDraw(string tournamentId);

        Task UpdateMainBraket(TournamentUpdateBracketDto tournament, IClientSessionHandle session);

        Task UpdateSilverCupBraket(TournamentUpdateBracketDto dto, IClientSessionHandle session);
    }
}
