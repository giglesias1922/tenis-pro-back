using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Repositories
{
    public class MatchesRepository:IMatch
    {
        private readonly IMongoCollection<Match> _matches;
        private readonly ITournament _tournamentsRepository;
        private readonly IUser _usersRepository;

        public MatchesRepository(IMongoDatabase database, ITournament tournamentsRepository, IUser usersRepository)
        {
            _matches = database.GetCollection<Match>("matches");
            _tournamentsRepository = tournamentsRepository;
            _usersRepository = usersRepository;
        }
        public async Task Post(Match match)
        {
            await _matches.InsertOneAsync(match);
        }

        public async Task<List<Match>> GetScheduledMatchesAsync()
        {
            return await _matches.Find(m => m.Status == MatchStatus.Scheduled)
                                     .Project<Match>(Builders<Match>.Projection.Exclude(m => m.History))
                                     .ToListAsync();
        }

        public async Task<List<MatchDto>> GetAll()
        {
            List<Match> list = await _matches.Find(t => true).ToListAsync();

            var tournaments = await _tournamentsRepository.GetTournamentsActives();

            var users = await _usersRepository.GetAll();

            var tournamentsMap = tournaments.ToDictionary(t => t.Id!, t => new { t.Description, t.TournamentType });

            var usersMap = users.ToDictionary(u => u.Id!, u => $"{u.LastName} {u.Name}");

            List<MatchDto> matches = new List<MatchDto>();

            foreach(Match match in list)
            {
                var tournament = tournamentsMap.ContainsKey(match.TournamentId)
                         ? tournamentsMap[match.TournamentId]
                         : null;

                string tournamentDesc = tournament?.Description ?? "N/A";
                TournamentType tournamentType = tournament?.TournamentType?? throw new ApplicationException("Tournament type not found");
                string tournamentTypeDesc = tournament?.TournamentType.ToString() ?? "Unknown";

                string playerAName = GetFormattedPlayers(match.PlayersA, tournamentType, usersMap);
                string playerBName = GetFormattedPlayers(match.PlayersB, tournamentType, usersMap);

                matches.Add(new MatchDto()
                {
                    TournamentDescription = tournamentDesc,
                    Type = tournamentType,
                    TournamentTypeDescription = tournamentTypeDesc,
                    PlayerAName = playerAName,
                    PlayerBName = playerBName,
                    Status = match.Status,
                });
            }

            return matches;
        }

        private string GetFormattedPlayers(List<string> playerIds, TournamentType tournamentType, Dictionary<string, string> usersMap)
        {
            var names = playerIds.Select(id => usersMap.ContainsKey(id) ? usersMap[id] : "N/A");

            if (tournamentType == TournamentType.Double)
                return string.Join(" / ", names);
            else // Single o default
                return names.FirstOrDefault() ?? "N/A";
        }

        public async Task<Match?> GetById(string id)
        {
            return await _matches.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<MatchHistory>> GetMatchHistoryAsync(string id)
        {
            var match = await _matches.Find(m => m.Id == id)
                                         .Project(m => m.History)
                                         .FirstOrDefaultAsync();
            return match ?? new List<MatchHistory>();
        }

        public async Task UpdateMatchStatusAsync(string id, MatchStatus status, string? notes)
        {
            var update = Builders<Match>.Update
                .Set(m => m.Status, status)
                .Push(m => m.History, new MatchHistory { Status = status, Notes = notes });

            await _matches.UpdateOneAsync(m => m.Id == id, update);
        }

        public async Task AddMatchResultAsync(string id, string result)
        {
            var update = Builders<Match>.Update
                .Set(m => m.Result, result)
                .Set(m => m.Status, MatchStatus.Completed)
                .Push(m => m.History, new MatchHistory { Status = MatchStatus.Completed });

            await _matches.UpdateOneAsync(m => m.Id == id, update);
        }

        public async Task Delete(string id)
        {
            await _matches.DeleteOneAsync(c => c.Id == id);
        }

    }
}
