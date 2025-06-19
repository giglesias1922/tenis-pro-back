using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models
{
    public class Ranking
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("playerid")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PlayerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("categoryid")]
        public string CategoryId { get; set; }

        [BsonElement("matchtype")]
        public MatchType MatchType { get; set; } // "single" o "double"

        public int Points { get; set; }

        public int TournamentsPlayed { get; set; }

        public int MatchesWon { get; set; }

        public int MatchesLost { get; set; }
    }

}

