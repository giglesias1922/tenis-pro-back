using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Match
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tournamentid")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string TournamentId { get; set; }

        [BsonElement("registrationId1")]
        public required string registrationId1 { get; set; } 

        [BsonElement("registrationId2")]
        public required  string registrationId2 { get; set; }

        [BsonElement("status")]
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        [BsonElement("history")]
        public List<MatchHistory> History { get; set; } = new();

        [BsonElement("result")]
        public string? Result { get; set; } // Ej: "6-3, 4-6, 10-7"

        [BsonElement("createdby")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CreatedBy { get; set; }

        [BsonElement("createdate")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public enum MatchStatus
        {
            Scheduled,
            InProgress,
            Completed,
            Suspended,
            Cancelled
        }

        public class MatchHistory
        {
            public DateTime Date { get; set; } = DateTime.UtcNow;
            public MatchStatus Status { get; set; }
            public string? Notes { get; set; }
        }
    }
}
