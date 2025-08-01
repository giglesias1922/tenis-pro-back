using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using tenis_pro_back.Models;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Controllers
{
    public class TournamentsTestController : Controller
    {
        [ApiController]
        [Route("api/testing/tournaments")]
        public class TournamentTestingController : ControllerBase
        {
            private readonly IMongoCollection<Match> _matches;
            private readonly IMongoCollection<Zone> _zones;
            private readonly IMongoCollection<Tournament> _tournaments;

            public TournamentTestingController(IMongoDatabase db)
            {
                _matches = db.GetCollection<Match>("Matches");
                _zones = db.GetCollection<Zone>("Zones");
                _tournaments = db.GetCollection<Tournament>("Tournaments");
            }

            // 1️⃣ Completar todos los partidos de una zona
            [HttpPost("{tournamentId}/zones/{zoneId}/complete")]
            public async Task<IActionResult> CompleteZone(string tournamentId, string zoneId)
            {
                var matches = await _matches.Find(m => m.TournamentId == tournamentId && m.ZoneId == zoneId).ToListAsync();
                foreach (var match in matches)
                {
                    if (match.Status != Match.MatchStatus.Completed)
                    {
                        match.Status = Match.MatchStatus.Completed;
                        match.Result = GenerateResult(match);
                        await _matches.ReplaceOneAsync(m => m.Id == match.Id, match);
                    }
                }
                return Ok($"Zona {zoneId} completada ({matches.Count} partidos)");
            }

            // 2️⃣ Completar todas las zonas y generar el Draw
            [HttpPost("{tournamentId}/zones/complete-all")]
            public async Task<IActionResult> CompleteAllZones(string tournamentId)
            {
                var tournament = await _tournaments.Find(t => t.Id == tournamentId).FirstOrDefaultAsync();
                if (tournament == null)
                    return NotFound("Torneo no encontrado");

                foreach (var zone in tournament.Zones)
                {
                    await CompleteZone(tournamentId, zone.Id);
                }

                return Ok("Todas las zonas completadas y Draw generado");
            }


            // 3️⃣ Completar un partido de eliminación directa (octavos, cuartos, semifinal, final)
            [HttpPost("{tournamentId}/matches/{matchId}/complete")]
            public async Task<IActionResult> CompleteMatch(string tournamentId, string matchId)
            {
                var match = await _matches.Find(m => m.Id == matchId).FirstOrDefaultAsync();
                if (match == null) return NotFound();

                if (!CanPlayMatch(tournamentId, match))
                    return BadRequest("No se puede jugar este partido todavía");

                match.Status = Match.MatchStatus.Completed;
                match.Result = GenerateResult(match);
                await _matches.ReplaceOneAsync(m => m.Id == match.Id, match);

                return Ok($"Match {matchId} completado");
            }

            // 4️⃣ Validar consistencia del torneo
            [HttpGet("{tournamentId}/validate")]
            public async Task<IActionResult> ValidateTournament(string tournamentId)
            {
                var tournament = await _tournaments.Find(t => t.Id == tournamentId).FirstOrDefaultAsync();
                if (tournament == null)
                    return NotFound("Torneo no encontrado");

                var matches = await _matches.Find(m => m.TournamentId == tournamentId).ToListAsync();

                // Verificar zonas incompletas
                var incompleteZones = tournament.Zones
                    .Where(z => matches.Any(m => m.ZoneId == z.Id && m.Status != Match.MatchStatus.Completed))
                    .Select(z => z.Id)
                    .ToList();

                // Verificar partidos duplicados
                var duplicates = matches
                    .GroupBy(m => new { m.Participant1Id, m.Participant2Id, m.ZoneId })
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                var errors = new List<string>();
                if (incompleteZones.Any())
                    errors.Add($"Zonas incompletas: {string.Join(", ", incompleteZones)}");
                if (duplicates.Any())
                    errors.Add("Partidos duplicados");

                return Ok(errors.Any() ? string.Join(" | ", errors) : "Torneo válido y consistente");
            }


            private MatchResult GenerateResult(Match match)
            {
                var random = new Random();
                var winner = random.Next(2) == 0 ? match.Participant1Id : match.Participant2Id;

                return new MatchResult
                {
                    Winner = winner,
                    Sets = new List<SetResult>
                {
                    new SetResult { PlayerAGames = 6, PlayerBGames = random.Next(0, 5), WinnerSet = winner },
                    new SetResult { PlayerAGames = 6, PlayerBGames = random.Next(0, 5), WinnerSet = winner }
                }
                };
            }

            private bool CanPlayMatch(string tournamentId, Match match)
            {
                var tournament = _tournaments
                    .Find(t => t.Id == tournamentId)
                    .FirstOrDefault();

                if (tournament == null)
                    return false;

                // Determinar el bracket (Main o SilverCup)
                var bracket = tournament.MainBracket?.Rounds
                    .FirstOrDefault(r => r.MatchIds.Contains(match.Id));

                if (bracket == null && tournament.SilverCupBracket != null)
                {
                    bracket = tournament.SilverCupBracket.Rounds
                        .FirstOrDefault(r => r.MatchIds.Contains(match.Id));
                }

                if (bracket == null)
                    return false;

                var currentRoundOrder = bracket.Order;

                // Si es la primera ronda, se puede jugar directamente
                if (currentRoundOrder <= 1)
                    return true;

                // Obtener todas las rondas previas
                var previousRoundMatches = _matches
                    .Find(m => m.TournamentId == tournamentId)
                    .ToList()
                    .Where(m =>
                        tournament.MainBracket?.Rounds.Any(r =>
                            r.Order == currentRoundOrder - 1 &&
                            r.MatchIds.Contains(m.Id)) == true
                        ||
                        tournament.SilverCupBracket?.Rounds.Any(r =>
                            r.Order == currentRoundOrder - 1 &&
                            r.MatchIds.Contains(m.Id)) == true
                    )
                    .ToList();

                // Solo se puede jugar si TODOS los partidos previos están completados
                return previousRoundMatches.All(m => m.Status == Match.MatchStatus.Completed);
            }

        }
    }
}