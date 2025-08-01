using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Collections.Generic;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Models.Enums;
using tenis_pro_back.Repositories;

namespace tenis_pro_back.Services
{
    public class TournamentProgressService : ITournamentProgressService
    {
        private readonly ITournament _tournamentRepository;
        private readonly IMatch _matchRepository;
        private readonly IMongoClient _mongoClient;
        private readonly ITournamentLog _tournamentLogRepository;

        public TournamentProgressService(ITournament tournamentRepository, IMatch matchRepository, IMongoClient mongoClient, ITournamentLog tournamentLogRepository)
        {
            _tournamentRepository = tournamentRepository;
            _matchRepository = matchRepository;
            _mongoClient = mongoClient;
            _tournamentLogRepository = tournamentLogRepository;
        }

        public string GetNextRoundName(string currentRound)
        {
            return currentRound switch
            {
                "Round of 16" => "Quarter-finals",
                "Quarter-finals" => "Semi-finals",
                "Semi-finals" => "Final",
                "Final" => "Winner",
                _ => "Winner"
            };
        }

        private async Task<List<ZoneStanding>> CalculateAllStandings(string tournamentId)
        {
            var tournament = await _tournamentRepository.GetById(tournamentId);
            if (tournament == null)
                throw new Exception("Tournament not found");

            var result = new List<ZoneStanding>();

            foreach (var zone in tournament.Zones)
            {
                var matches = await _matchRepository.GetMatchesByZone(tournamentId, zone.Id);
                var standings = CalculateStandings(zone.ParticipantIds, matches, zone.Id);
                result.AddRange(standings);
            }

            return result;
        }

        private List<ZoneStanding> CalculateStandings(List<string> participantIds, List<Match> matches, string zoneId)
        {
            var standings = participantIds.ToDictionary(id => id, id => new ZoneStanding
            {
                ParticipantId = id,
                Wins = 0,
                Losses = 0,
                SetsWon = 0,
                SetsLost = 0,
                GamesWon = 0,
                GamesLost = 0,
                ZoneId = zoneId
            });

            foreach (var match in matches.Where(m => m.Status == Match.MatchStatus.Completed && m.Result != null))
            {
                var p1 = match.Participant1Id!;
                var p2 = match.Participant2Id!;
                var winner = match.Result.Winner;

                if (winner == p1)
                {
                    standings[p1].Wins++;
                    standings[p2].Losses++;
                }
                else
                {
                    standings[p2].Wins++;
                    standings[p1].Losses++;
                }

                foreach (var set in match.Result.Sets)
                {
                    standings[p1].GamesWon += set.PlayerAGames;
                    standings[p1].GamesLost += set.PlayerBGames;
                    standings[p2].GamesWon += set.PlayerBGames;
                    standings[p2].GamesLost += set.PlayerAGames;

                    if (set.PlayerAGames > set.PlayerBGames)
                    {
                        standings[p1].SetsWon++;
                        standings[p2].SetsLost++;
                    }
                    else
                    {
                        standings[p2].SetsWon++;
                        standings[p1].SetsLost++;
                    }
                }
            }

            return standings.Values.ToList();
        }

        public async Task RegisterMatchResult(string id, MatchResultDto result)
        {
            var match = await _matchRepository.GetById(id);
            if (match == null)
                throw new Exception("Match not found");

            var tournament = await _tournamentRepository.GetById(match.TournamentId);
            if (tournament == null)
                throw new Exception("Tournament not found");

            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();

            try
            {
                await _matchRepository.AddMatchResult(id, result, session);

                if (!string.IsNullOrEmpty(match.ZoneId))
                {
                    bool allCompleted = await AllGroupMatchesCompleted(match.TournamentId, session);
                    if (allCompleted)
                    {
                        var standings = await CalculateAllStandings(match.TournamentId);

                        await BuildBracketStructure(match.TournamentId, standings, session, false);

                        if (tournament.IncludePlata)
                            await BuildBracketStructure(match.TournamentId, standings, session, true);
                    }
                }
                else
                {
                    await TryAdvanceBracket(match.TournamentId, match.RoundName!, session, match.BracketType);
                }

                await session.CommitTransactionAsync();
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        private async Task BuildBracketStructure(string tournamentId, List<ZoneStanding> standings, IClientSessionHandle session, bool isSilver)
        {
            var tournament = await _tournamentRepository.GetById(tournamentId);
            if (tournament == null)
                throw new Exception("Tournament not found");

            var players = isSilver
                ? standings.GroupBy(s => s.ZoneId).SelectMany(g => g.Skip(tournament.QualifiersPerZone)).Select(s => s.ParticipantId).ToList()
                : standings.GroupBy(s => s.ZoneId).SelectMany(g => g.Take(tournament.QualifiersPerZone)).Select(s => s.ParticipantId).ToList();

            if (players.Count < 2) return;

            // ✅ Calcular tamaño del cuadro
            int bracketSize = 1;
            while (bracketSize < players.Count)
                bracketSize *= 2;

            int rounds = (int)Math.Log2(bracketSize);

            // ✅ Crear estructura del bracket
            var bracket = new Bracket();
            string currentRoundName = bracketSize switch
            {
                2 => "Final",
                4 => "Semi-finals",
                8 => "Quarter-finals",
                16 => "Round of 16",
                _ => "Elimination"
            };

            // 🔹 1️⃣ Crear la primera ronda con partidos
            var firstRound = new Round { Name = currentRoundName, Order = 1 };
            var queue = new Queue<string>(players);

            while (queue.Count > 0)
            {
                var p1 = queue.Dequeue();
                var p2 = queue.Count > 0 ? queue.Dequeue() : null;

                var match = new Match
                {
                    TournamentId = tournament.Id!,
                    Participant1Id = p1,
                    Participant2Id = p2,
                    RoundName = currentRoundName,
                    Status = Match.MatchStatus.Scheduled,
                    BracketType = isSilver ? "SilverCup" : "Main"
                };

                var created = await _matchRepository.CreateMatch(match, session);
                firstRound.MatchIds.Add(created.Id!);
            }

            bracket.Rounds.Add(firstRound);

            // 🔹 2️⃣ Crear rondas vacías (octavos → cuartos → semis → final)
            for (int r = 2; r <= rounds; r++)
            {
                currentRoundName = GetNextRoundName(currentRoundName);
                bracket.Rounds.Add(new Round { Name = currentRoundName, Order = r });
            }

            var dto = new TournamentUpdateBracketDto
            {
                Id = tournament.Id,
                Status = TournamentStatusEnum.InProgress,
                DrawBracket = bracket
            };

            if (isSilver)
                await _tournamentRepository.UpdateSilverCupBraket(dto, session);
            else
                await _tournamentRepository.UpdateMainBraket(dto, session);
        }

        private async Task TryAdvanceBracket(string tournamentId, string currentRound, IClientSessionHandle session, string bracketType)
        {
            var tournament = await _tournamentRepository.GetById(tournamentId);
            var bracket = bracketType == "SilverCup" ? tournament.SilverCupBracket : tournament.MainBracket;

            var round = bracket.Rounds.FirstOrDefault(r => r.Name == currentRound);
            if (round == null) return;

            var matches = await _matchRepository.GetMatchesByIdsAsync(round.MatchIds);
            if (matches.Any(m => m.Status != Match.MatchStatus.Completed)) return;

            var winners = matches.Select(m => m.Result!.Winner).ToList();
            if (winners.Count < 2)
            {
                tournament.Status = TournamentStatusEnum.Completed;
                await _tournamentRepository.UpdateStatus(tournamentId, TournamentStatusEnum.Completed, session);
                return;
            }

            var nextRoundName = GetNextRoundName(currentRound);
            var nextRound = bracket.Rounds.FirstOrDefault(r => r.Name == nextRoundName);
            if (nextRound == null) return;

            for (int i = 0; i < winners.Count; i += 2)
            {
                var match = new Match
                {
                    TournamentId = tournament.Id!,
                    Participant1Id = winners[i],
                    Participant2Id = (i + 1 < winners.Count) ? winners[i + 1] : null,
                    RoundName = nextRoundName,
                    Status = Match.MatchStatus.Scheduled,
                    BracketType = bracketType
                };

                var created = await _matchRepository.CreateMatch(match, session);
                nextRound.MatchIds.Add(created.Id!);
            }

            var dto = new TournamentUpdateBracketDto
            {
                Id = tournament.Id,
                Status = tournament.Status,
                DrawBracket = bracket
            };

            if (bracketType == "SilverCup")
                await _tournamentRepository.UpdateSilverCupBraket(dto, session);
            else
                await _tournamentRepository.UpdateMainBraket(dto, session);
        }

        private async Task<bool> AllGroupMatchesCompleted(string tournamentId, IClientSessionHandle session)
        {
            var matches = await _matchRepository.GetMatchesByTournament(tournamentId, session);
            return matches.All(m => m.Status == Match.MatchStatus.Completed);
        }
    }
}
