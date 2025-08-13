using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Text.Json;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Repositories
{
    public class MatchesRepository:IMatch
    {
        private readonly IMongoCollection<Match> _matchesCollection;
        private readonly ITournament _tournamentsRepository;
        private readonly IUser _usersRepository;

        public MatchesRepository(IMongoDatabase database, ITournament tournamentsRepository, IUser usersRepository)
        {
            _matchesCollection = database.GetCollection<Match>("Matches");
            _tournamentsRepository = tournamentsRepository;
            _usersRepository = usersRepository;
        }
        public async Task Post(Match match)
        {
            await _matchesCollection.InsertOneAsync(match);
        }

        public async Task<List<Match>> GetMatchesByIdsAsync(List<string> matchIds)
        {
            var matches = await _matchesCollection
                .Find(m => matchIds.Contains(m.Id))
                .ToListAsync();

            return matches;
        }

        public async Task<List<Match>> GetScheduledMatchesAsync()
        {
            return await _matchesCollection.Find(m => m.Status == MatchStatus.Scheduled)
                                     .Project<Match>(Builders<Match>.Projection.Exclude(m => m.History))
                                     .ToListAsync();
        }

        public async Task<List<MatchDto>> GetAll()
        {
            List<Match> matches = await _matchesCollection.Find(t => true).ToListAsync();

            List<MatchDto> matchesDto = new List<MatchDto>();

            foreach(Match match in matches)
            {
                var tournament = await _tournamentsRepository.GetById(match.TournamentId);

                string tournamentDesc = tournament?.Description ?? "N/A";
                Models.Enums.TournamentTypeEnum tournamentType = tournament?.TournamentType?? throw new ApplicationException("Tournament type not found");
                string tournamentTypeDesc = tournament?.TournamentType.ToString() ?? "Unknown";
                
                var participant1 = tournament?.Participants.FirstOrDefault(p => p.Id == match.Participant1Id);
                var participant2 = tournament?.Participants.FirstOrDefault(p => p.Id == match.Participant2Id);

                string playerAName = participant1?.DisplayName ?? "N/A";
                string playerBName = participant2?.DisplayName ?? "N/A";
                
                var tournamentDto = await _tournamentsRepository.GetDtoById(match.TournamentId);

                matchesDto.Add(new MatchDto()
                {
                    Id = match.Id,
                    TournamentDescription = tournamentDesc,
                    Type = tournamentType,
                    TournamentTypeDescription = tournamentTypeDesc,
                    PlayerAName = playerAName,
                    PlayerBName = playerBName,
                    Status = match.Status,
                    StatusDescription = match.Status.ToString(),
                    LocationDescription = tournamentDto.LocationDescription,
                    CategoryDescription = tournamentDto.CategoryDescription
                });
            }

            return matchesDto;
        }

        public async Task<MatchDto?> GetById(string id)
        {
            var match = await _matchesCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (match == null) return null;

            var tournament = await _tournamentsRepository.GetById(match.TournamentId);
            
            if (tournament == null) throw new ApplicationException("Tournament not found for this match");
            
            string tournamentDesc = tournament.Description;
            Models.Enums.TournamentTypeEnum tournamentType = tournament.TournamentType;
            string tournamentTypeDesc = tournament.TournamentType.ToString();

            var participant1 = tournament.Participants.FirstOrDefault(p => p.Id == match.Participant1Id);
            var participant2 = tournament.Participants.FirstOrDefault(p => p.Id == match.Participant2Id);

            var tournamentDto = await _tournamentsRepository.GetDtoById(match.TournamentId);
            if (tournamentDto == null) throw new ApplicationException("Tournament DTO not found");

            return new MatchDto()
            {
                Id = id,
                TournamentDescription = tournamentDesc,
                Type = tournamentType,
                TournamentTypeDescription = tournamentTypeDesc,
                PlayerAName = participant1?.DisplayName ?? "N/A",
                PlayerBName = participant2?.DisplayName ?? "N/A",
                Participant1Id = participant1?.Id,
                Participant2Id = participant2?.Id,
                Status = match.Status,
                LocationDescription = tournamentDto.LocationDescription,
                CategoryDescription = tournamentDto.CategoryDescription,
                ZoneId = match.ZoneId,
                TournamentId = match.TournamentId,
                RoundName = match.RoundName,
            };
        }

        

        public async Task<List<Match>> GetMatchesByZone(string tournamentId, string zoneId)
        {
            var filter = Builders<Match>.Filter.And(
                Builders<Match>.Filter.Eq(m => m.TournamentId, tournamentId),
                Builders<Match>.Filter.Eq(m => m.ZoneId, zoneId)
            );

            return await _matchesCollection.Find(filter).ToListAsync();
        }



        public async Task<List<Match>> GetMatchesByTournament(string tournamentId, IClientSessionHandle? session = null)
        {
            var filter = Builders<Match>.Filter.Eq(m => m.TournamentId, tournamentId);

            if (session != null)
                return await _matchesCollection.Find(session, filter).ToListAsync();
            else
                return await _matchesCollection.Find(filter).ToListAsync();
        }


        public async Task<List<MatchHistory>> GetMatchHistoryAsync(string id)
        {
            var matches = await _matchesCollection.Find(m => m.Id == id)
                                         .Project(m => m.History)
                                         .FirstOrDefaultAsync();

            List<MatchHistory> list = new();

            foreach(var match in matches )
            {
                list.Add(new MatchHistory
                {
                    Date = match.Date,
                    Notes = match.Notes,
                    Status = match.Status

                });
            }

            var sortedHistory = list.OrderBy(h => h.Date).ToList();

            return sortedHistory;
        }

        public async Task UpdateMatchStatusAsync(string id, MatchStatus status, string? notes)
        {
            var update = Builders<Match>.Update
                .Set(m => m.Status, status)
                .Push(m => m.History, new MatchHistory { Status = status, Notes = notes });

            await _matchesCollection.UpdateOneAsync(m => m.Id == id, update);
        }

        
        

        public async Task Delete(string id)
        {
            await _matchesCollection.DeleteOneAsync(c => c.Id == id);
        }


        public async Task<Match> CreateMatch(Models.Match match, IClientSessionHandle session)
        {
            await _matchesCollection.InsertOneAsync(session, match);

            return match;
        }

        public async Task AddMatchResult(string id, MatchResultDto dto,  IClientSessionHandle session)
        {
            var update = Builders<Match>.Update;
            var updates = new List<UpdateDefinition<Match>>();

            if (dto.Winner != null)
            {
                var matchResult = new Match.MatchResult
                {                    
                    Winner = dto.Winner,
                    Sets = dto.Sets.Select(s => new Match.SetResult
                    {
                        PlayerAGames = s.PlayerA_games,
                        PlayerBGames = s.PlayerB_games,
                        Tiebreak = s.Tiebreak,
                        WinnerSet = string.IsNullOrWhiteSpace(s.WinnerSet) ? null : s.WinnerSet
                    }).ToList(),
                    Points = dto.Points
                };

                updates.Add(update.Set(m => m.Result, matchResult));
                updates.Add(update.Set(m => m.Status, Match.MatchStatus.Completed));
                updates.Add(update.Push(m => m.History, new Match.MatchHistory
                {
                    Date = DateTime.UtcNow,
                    Status = Match.MatchStatus.Completed,
                    Notes = "Result updated"
                }));
            }

            
            if (updates.Count == 0)
                return; // Nada para actualizar

            var combined = update.Combine(updates);
            await _matchesCollection.UpdateOneAsync(session,m => m.Id == id, combined);
        }
    }
}
