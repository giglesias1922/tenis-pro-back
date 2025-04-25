using Microsoft.Extensions.Diagnostics.HealthChecks;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Models.Dto
{
    public class MatchDto
    {
        public required string TournamentDescription { get; set; }
        public required string PlayerAName { get; set; }
        public required string PlayerBName { get; set; }
        public MatchStatus Status { get; set; }

        public TournamentType Type { get; set; }
        public required string TournamentTypeDescription { get; set; }

    }
}
