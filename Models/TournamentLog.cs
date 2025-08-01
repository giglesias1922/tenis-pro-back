using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models
{
    public class TournamentLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("tournament_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TournamentId { get; set; }

        [BsonElement("match_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MatchId { get; set; }

        [BsonElement("participant1_player_ids")]
        public List<string> Participant1PlayerIds { get; set; } = new();

        [BsonElement("participant2_player_ids")]
        public List<string> Participant2PlayerIds { get; set; } = new();

        [BsonElement("participant1_display")]
        public string Participant1Display { get; set; }

        [BsonElement("participant2_display")]
        public string Participant2Display { get; set; }

        [BsonElement("result")]
        public string Result { get; set; } // Ej: "6-3, 4-6, 10-7"

        [BsonElement("sets")]
        public List<Match.SetResult> Sets { get; set; } = new();

        [BsonElement("round_name")]
        public string RoundName { get; set; }

        [BsonElement("stage")]
        public string Stage { get; set; } // "Zone", "Main", "SilverCup"

        [BsonElement("is_double")]
        public bool IsDouble { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
