using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class User
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("lastname")]
        public required string LastName { get; set; }

        [BsonElement("phone1")]
		public string? Phone1 { get; set; }

		[BsonElement("phone2")]
		public string? Phone2 { get; set; }

		[BsonElement("email")]
		public string? Email { get; set; }

		[BsonElement("categoryid")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? CategoryId { get; set; } //id categoria actual

		[BsonElement("image")]
		public string? Image { get; set; }

		[BsonElement("profileid")]
		[BsonRepresentation(BsonType.ObjectId)]
		public required string ProfileId{ get; set; }  // ID profile

		[BsonElement("active")]
		public bool Active { get; set; } = true;

        [BsonElement("birthdate")]
        public string? BirthDate { get; set; }
    }
}