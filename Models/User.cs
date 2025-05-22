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

		[BsonElement("comment")]
		public string? Comment { get; set; }

		[BsonElement("email")]
		public required string Email { get; set; }

		[BsonElement("categoryid")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? CategoryId { get; set; } //id categoria actual

		[BsonElement("image")]
		public string? Image { get; set; }

		[BsonElement("profileid")]
		[BsonRepresentation(BsonType.ObjectId)]
		public required string ProfileId{ get; set; }  // ID profile

		[BsonElement("status")]
		public required UserStatus Status{ get; set; }

        [BsonElement("birthdate")]
        public string? BirthDate { get; set; }

        [BsonElement("identification")]
        public string? Identification { get; set; }

        [BsonElement("password")]
        public string? Password { get; set; }
    }

	public enum UserStatus  :Int16
	{
		Disabled=  0,
		Enabled= 1,
		PendingActivation=2,
        ChangePassword=3
    }

}