using Microsoft.Extensions.Diagnostics.HealthChecks;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Models.Dto
{
    public class MatchDto
    {
        public required string Id { get; set; }
        public required string TournamentDescription { get; set; }
        public required string PlayerAName { get; set; }
        public required string PlayerBName { get; set; }
        public MatchStatus Status { get; set; }

        public Models.Enums.TournamentTypeEnum Type { get; set; }
        public required string TournamentTypeDescription { get; set; }
        public string? CategoryDescription { get; set; }
        public string? LocationDescription { get; set; }
        public string? Participant1Id { get; set; }
        public string? Participant2Id { get; set; }
        public string? StatusDescription { get; set; }

        public string? ZoneId { get; set; }
        public string? TournamentId { get; set; }
        public string? RoundName { get; set; }
        public string? BracketType { get; set; }
    }
}
