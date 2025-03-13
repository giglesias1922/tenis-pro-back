using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Profile
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("name")]
        public required string Name { get; set; }

		[BsonElement("functionalities")]
		[BsonRepresentation(BsonType.ObjectId)]
		public required List<string>? Functionalities { get; set; }  // IDs de las funcionalidades

		[BsonElement("esadmin")]
		public required bool EsAdmin { get; set; }

    }
}
