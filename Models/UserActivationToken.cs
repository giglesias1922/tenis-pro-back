using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class UserActivationToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userid")]
        [BsonRepresentation(BsonType.ObjectId)]        
        public required string UserId { get; set; }
        [BsonElement("token")]
        public required string Token { get; set; }
        [BsonElement("expiration")]
        public required DateTime Expiration { get; set; }
    }
}
