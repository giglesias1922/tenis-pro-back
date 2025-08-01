using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models
{
    public class Zone
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("participants")]
        public List<string> ParticipantIds { get; set; } = new();

        [BsonElement("standings")]
        public List<ZoneStanding> Standings { get; set; } = new(); // 🆕
    }
}
