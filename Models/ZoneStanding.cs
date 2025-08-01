using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models
{
    public class ZoneStanding
    {
        [BsonElement("participantId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ParticipantId { get; set; }

        [BsonElement("wins")]
        public int Wins { get; set; }

        [BsonElement("losses")]
        public int Losses { get; set; }

        [BsonElement("setsWon")]
        public int SetsWon { get; set; }

        [BsonElement("setsLost")]
        public int SetsLost { get; set; }

        [BsonElement("gamesWon")]
        public int GamesWon { get; set; }

        [BsonElement("gamesLost")]
        public int GamesLost { get; set; }

        [BsonElement("position")]
        public int Position { get; set; }

        [BsonElement("zoneId")]
        public string ZoneId { get; set; }

        [BsonElement("playerId")]
        public string PlayerId { get; set; }
    }
}
