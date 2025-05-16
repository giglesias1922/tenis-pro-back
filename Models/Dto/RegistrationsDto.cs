using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models.Dto
{
    public class RegistrationsDto
    {
        public string? Id { get; set; }
        public required string TournamentId { get; set; }  // IDs del torneo

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedByName { get; set; }
        public required string DisplayName { get; set; }
        public string? TournamentDescription { get; set; }
    }
}
