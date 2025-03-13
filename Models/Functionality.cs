using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Functionality
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("name")]
        public string Name { get; set; }

		[BsonElement("code")]
		public required string Code { get; set; }


	}
}
