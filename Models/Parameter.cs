using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models
{
    public class Parameter
    {
        [BsonId]
        public required string Id { get; set; }

        [BsonElement("value")]
        public required string Value{ get; set; }

        [BsonElement("description")]
        public required string Description { get; set; }

        [BsonElement("modifdate")]
        public DateTime? ModifDate { get; set; }

        [BsonElement("modifuser")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ModifUser{ get; set; }

    }
}
