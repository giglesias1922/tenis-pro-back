using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models
{
    public class UserToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // Para que sea un GUID como string en la colección
        public Guid Token { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)] // Suponiendo que el UserId es un ObjectId
        public string UserId { get; set; }

        [BsonElement("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [BsonElement("used")]
        public bool Used { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
