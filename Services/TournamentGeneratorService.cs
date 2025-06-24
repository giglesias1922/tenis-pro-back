using MongoDB.Bson;
using System.Linq;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Services
{
    public class TournamentGeneratorService : ITournamentGeneratorService
    {
        private readonly ITournament _tournamentRepository;
        private readonly IMatch _matchRepository;

        public TournamentGeneratorService(ITournament tournamentRepository, IMatch matchRepository)
        {
            _tournamentRepository = tournamentRepository;
            _matchRepository = matchRepository;
        }

        public async Task<Tournament> GenerateDraw(string tournamentId)
        {
            var tournament = await _tournamentRepository.GetById(tournamentId);
            if (tournament == null)
            {
                throw new Exception("Tournament not found");
            }

            if (tournament.Status != Models.Enums.TournamentStatusEnum.Programming)
                throw new Exception("The tournament is not in a state to be programmed.");

            var participants = tournament.Participants.OrderBy(p => p.Ranking).ToList();
            if (participants.Count < 2)
            {
                throw new Exception("Not enough participants to generate the draw.");
            }

            // 1. Calculate number of zones
            var numZones = (int)Math.Ceiling((double)participants.Count / tournament.PlayersPerZone);

            // 2. Identify seeds
            var seeds = participants.Take(numZones).ToList();
            var rest = participants.Skip(numZones).ToList();

            // 3. Shuffle the rest of the players
            var random = new Random();
            rest = rest.OrderBy(x => random.Next()).ToList();

            // 4. Create zones and assign seeds
            tournament.Zones.Clear();
            for (int i = 0; i < numZones; i++)
            {
                tournament.Zones.Add(new Zone
                {
                    Name = $"Zone {Convert.ToChar(65 + i)}",
                    ParticipantIds = new List<string> { seeds[i].Id }
                });
            }

            // 5. Distribute the rest of the players
            int currentZoneIndex = 0;
            foreach (var player in rest)
            {
                while (tournament.Zones[currentZoneIndex].ParticipantIds.Count >= tournament.PlayersPerZone)
                {
                    currentZoneIndex = (currentZoneIndex + 1) % numZones;
                }
                tournament.Zones[currentZoneIndex].ParticipantIds.Add(player.Id);
            }
            
            // 6. Generate matches for each zone (round-robin)
            tournament.Status = Models.Enums.TournamentStatusEnum.Initiated;
            await GenerateMatchesForZones(tournament);

            await _tournamentRepository.Put(tournament.Id, tournament);
            return tournament;
        }

        private async Task GenerateMatchesForZones(Tournament tournament)
        {
            foreach (var zone in tournament.Zones)
            {
                var participantsInZone = zone.ParticipantIds;
                for (int i = 0; i < participantsInZone.Count; i++)
                {
                    for (int j = i + 1; j < participantsInZone.Count; j++)
                    {
                        var match = new Match
                        {
                            TournamentId = tournament.Id,
                            Participant1Id = participantsInZone[i],
                            Participant2Id = participantsInZone[j],
                            ZoneId = zone.Id,
                            RoundName = zone.Name,
                            Status = Models.Match.MatchStatus.Scheduled
                        };
                        await _matchRepository.CreateMatch(match);
                    }
                }
            }
        }
    }
} 