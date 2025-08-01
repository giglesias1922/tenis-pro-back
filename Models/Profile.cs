using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Profile
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonElement("name")]
        public required string Name { get; set; }

		[BsonElement("functionalities")]
		[BsonRepresentation(BsonType.ObjectId)]
		public List<string>? Functionalities { get; set; } = new List<string>();  // IDs de las funcionalidades

		[BsonElement("type")]
		public required ProfileType Type { get; set; }

		public enum ProfileType : Int16
		{
            Admin=1,
            Organizer = 2,
            Players = 3
        }
    }
}
