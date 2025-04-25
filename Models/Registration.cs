using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Registration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tournamentid")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string TournamentId { get; set; }  // IDs del torneo

        [BsonElement("registrationid")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string RegistrationId { get; set; }  // Uno o dos jugadores

        [BsonElement("createdby")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CreatedBy { get; set; }

        [BsonElement("createdate")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("displayname")]
        public required string DisplayName { get; set; }
    }
}
