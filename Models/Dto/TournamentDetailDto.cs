using MongoDB.Bson.Serialization.Attributes;
using tenis_pro_back.Models;

namespace tenis_pro_back.Models.Dto
{
    public class TournamentDetailDto
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public DateTime? CloseDate { get; set; } //cierre de inscripcion
        public DateTime? InitialDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Models.Enums.TournamentStatusEnum? Status { get; set; }
        public string? CategoryDescription { get; set; }
        public string? LocationDescription { get; set; }
        public string? CategoryId { get; set; }
        public string? LocationId { get; set; }
        public string? StatusName { get; set; }
        public Models.Enums.TournamentTypeEnum? TournamentType { get; set; }
        public string ? TournamentTypeDescription { get; set; }
        public string? Image { get; set; }
        
        // Propiedades para participantes y configuración
        public List<Participant> Participants { get; set; } = new();
        public int PlayersPerZone { get; set; } = 4;
        public int QualifiersPerZone { get; set; } = 2;
        public bool IncludePlata { get; set; } = false;
        public List<Zone> Zones { get; set; } = new();
        public Bracket? MainBracket { get; set; }
        public Bracket? SilverCupBracket { get; set; }
    }
}
