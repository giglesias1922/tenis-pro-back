using MongoDB.Driver;
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
        private readonly IRegistration _registrationRepository;
        private readonly ITournament _tournamentsRepository;
        private readonly IUser _usersRepository;

        public MatchesRepository(IMongoDatabase database, ITournament tournamentsRepository, IUser usersRepository, IRegistration registrationRepository)
        {
            _matchesCollection = database.GetCollection<Match>("Matches");
            _tournamentsRepository = tournamentsRepository;
            _usersRepository = usersRepository;
            _registrationRepository = registrationRepository;
        }
        public async Task Post(Match match)
        {
            await _matchesCollection.InsertOneAsync(match);
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

            var tournaments = await _tournamentsRepository.GetAll();

            List<MatchDto> matchesDto = new List<MatchDto>();

            foreach(Match match in matches)
            {
                var tournament = tournaments.FirstOrDefault(w => w.Id == match.TournamentId);

                string tournamentDesc = tournament?.Description ?? "N/A";
                TournamentType tournamentType = tournament?.TournamentType?? throw new ApplicationException("Tournament type not found");
                string tournamentTypeDesc = tournament?.TournamentType.ToString() ?? "Unknown";

                Registration registration1 = await _registrationRepository.GetById(match.registrations[0]);
                Registration registration2 = await _registrationRepository.GetById(match.registrations[1]);

                string playerAName = registration1?.DisplayName ?? "Desconocido";
                string playerBName = registration2?.DisplayName ?? "Desconocido";

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
                    LocationDescription = tournament.LocationDescription,
                    CategoryDescription = tournament.CategoryDescription
                });
            }

            return matchesDto;
        }

        public async Task<MatchDto?> GetById(string id)
        {
            var match = await _matchesCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            

            var tournament = await _tournamentsRepository.GetById(match.TournamentId);

            string tournamentDesc = tournament?.Description ?? "N/A";
                TournamentType tournamentType = tournament?.TournamentType ?? throw new ApplicationException("Tournament type not found");
                string tournamentTypeDesc = tournament?.TournamentType.ToString() ?? "Unknown";

            Registration registracion1 = await _registrationRepository.GetById(match.registrations[0]);
            Registration registracion2 = await _registrationRepository.GetById(match.registrations[1]);


            return new MatchDto()
            {
                Id = id,
                TournamentDescription = tournamentDesc,
                Type = tournamentType,
                TournamentTypeDescription = tournamentTypeDesc,
                PlayerAName = registracion1.DisplayName,
                PlayerBName = registracion2.DisplayName,
                RegistrationAId = registracion1.Id,
                RegistrationBId = registracion2.Id,
                Status = match.Status,
                LocationDescription = tournament.LocationDescription,
                CategoryDescription = tournament.CategoryDescription
            };
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

        public async Task AddMatchResultAsync(string id, MatchResultDto result)
        {
            var filter = Builders<Match>.Filter.Eq(m => m.Id, id);

            var resultField = new StringFieldDefinition<Match, MatchResultDto>("Result");  // Definir el campo explícitamente

            var update = Builders<Match>.Update
                .Set(resultField, result)  // Usa el FieldDefinition aquí
                .Set("Status", Match.MatchStatus.Completed)
                .Push("History", new Match.MatchHistory { Status = Match.MatchStatus.Completed });

            await _matchesCollection.UpdateOneAsync(filter, update);
        }


        public async Task Delete(string id)
        {
            await _matchesCollection.DeleteOneAsync(c => c.Id == id);
        }

    }
}
