using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models.Dto
{
    public class TournamentUpdateDto
    {
        public string? Id { get; set; }
        public required string Description { get; set; }
        public DateTime? CloseDate { get; set; } //cierre de inscripcion
        public DateTime? InitialDate { get; set; }
        public DateTime? EndDate { get; set; }
        public required string LocationId { get; set; }  // IDs de las locaciones
        public required string CategoryId { get; set; }  // IDs de las categorías
        public required Models.Enums.TournamentTypeEnum TournamentType { get; set; }  // single /doble
        public string? Image { get; set; }        
    }
}
