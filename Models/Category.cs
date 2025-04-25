namespace tenis_pro_back.Models
{
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;

	public class Category
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonElement("description")]
		public string Description { get; set; }


        [BsonElement("active")]
        public bool Active { get; set; } = true;
    }
}
