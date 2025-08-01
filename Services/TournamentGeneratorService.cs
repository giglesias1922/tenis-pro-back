using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Models.Enums;

namespace tenis_pro_back.Services
{
    public class TournamentGeneratorService : ITournamentGeneratorService
    {
        private readonly ITournament _tournamentRepository;
        private readonly IMatch _matchRepository;
        private readonly IMongoClient _mongoClient;

        public TournamentGeneratorService(
            ITournament tournamentRepository,
            IMatch matchRepository,
            IMongoClient mongoClient)
        {
            _tournamentRepository = tournamentRepository;
            _matchRepository = matchRepository;
            _mongoClient = mongoClient;
        }

        public async Task<Tournament> GenerateDraw(string tournamentId, DrawConfigurationDto config)
        {
            var tournament = await ValidateTournamentState(tournamentId);

            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var zones = CalculateZones(tournament.Participants, config);

                await SaveTournamentDraw(tournamentId, zones, config, session);
                await GenerateRoundRobinMatches(tournamentId, zones, session);

                await session.CommitTransactionAsync();

                return await _tournamentRepository.GetById(tournamentId);
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        private async Task<Tournament> ValidateTournamentState(string tournamentId)
        {
            var tournament = await _tournamentRepository.GetById(tournamentId);
            if (tournament == null)
                throw new Exception("Tournament not found");

            if (tournament.Status != TournamentStatusEnum.Programming)
                throw new Exception("Tournament is not in Programming state.");

            if (tournament.Participants.Count < 2)
                throw new Exception("Not enough participants.");

            return tournament;
        }

        private List<Zone> CalculateZones(List<Participant> participants, DrawConfigurationDto config)
        {
            var ordered = participants.OrderBy(p => p.Ranking).ToList();
            int numZones = (int)Math.Ceiling((double)ordered.Count / config.PlayersPerZone);

            var seeds = ordered.Take(numZones).ToList();
            var rest = ordered.Skip(numZones).OrderBy(x => Guid.NewGuid()).ToList();

            var zones = new List<Zone>();
            for (int i = 0; i < numZones; i++)
            {
                zones.Add(new Zone
                {
                    Name = $"Zone {Convert.ToChar(65 + i)}",
                    ParticipantIds = new List<string> { seeds[i].Id }
                });
            }

            int index = 0;
            foreach (var player in rest)
            {
                while (zones[index].ParticipantIds.Count >= config.PlayersPerZone)
                    index = (index + 1) % numZones;

                zones[index].ParticipantIds.Add(player.Id);
            }

            return zones;
        }

        private async Task SaveTournamentDraw(
            string tournamentId,
            List<Zone> zones,
            DrawConfigurationDto config,
            IClientSessionHandle session)
        {
            var update = new TournamentUpdateDrawDto
            {
                Id = tournamentId,
                IncludePlata = config.IncludePlata,
                PlayersPerZone = config.PlayersPerZone,
                QualifiersPerZone = config.QualifiersPerZone,
                Zones = zones,
                Status = TournamentStatusEnum.Initiated
            };

            await _tournamentRepository.UpdateDraw(update, session);
        }

        private async Task GenerateRoundRobinMatches(
            string tournamentId,
            List<Zone> zones,
            IClientSessionHandle session)
        {
            foreach (var zone in zones)
            {
                var players = zone.ParticipantIds;
                for (int i = 0; i < players.Count; i++)
                {
                    for (int j = i + 1; j < players.Count; j++)
                    {
                        var match = new Match
                        {
                            TournamentId = tournamentId,
                            Participant1Id = players[i],
                            Participant2Id = players[j],
                            ZoneId = zone.Id,
                            RoundName = zone.Name,
                            Status = Match.MatchStatus.Pending
                        };
                        await _matchRepository.CreateMatch(match, session);
                    }
                }
            }
        }
    }
}