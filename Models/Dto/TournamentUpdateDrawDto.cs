using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models.Dto
{
    public class TournamentUpdateDrawDto
    {
        public string? Id { get; set; }
        public Models.Enums.TournamentStatusEnum Status { get; set; }
        public int PlayersPerZone { get; set; } // jugadores por zona
        public int QualifiersPerZone { get; set; } // cuántos clasifican por zona
        public bool IncludePlata { get; set; } = false;
        public List<Zone> Zones { get; set; } = new();
    }
}
