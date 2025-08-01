using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Repositories
{
    public class TournamentLogRepository: ITournamentLog
    {
        private readonly IMongoCollection<TournamentLog> _logCollection;

        public TournamentLogRepository(IMongoDatabase database)
        {
            _logCollection = database.GetCollection<TournamentLog>("tournament_logs");
        }

        public async Task AddLog(TournamentLog log)
        {
            await _logCollection.InsertOneAsync(log);
        }

        public async Task<List<TournamentLog>> GetLogsByTournament(string tournamentId)
        {
            return await _logCollection.Find(l => l.TournamentId == tournamentId)
                                 .SortBy(l => l.CreatedAt)
                                 .ToListAsync();
        }
    }
}
