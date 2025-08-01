using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Location
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonElement("name")]
		public string name { get; set; }


		[BsonElement("address")]
		public string? address { get; set; }


		[BsonElement("phone")]
		public string? Phone { get; set; }

		[BsonElement("comments")]
		public string? Comments { get; set; }


        [BsonElement("active")]
        public bool Active { get; set; } = true;

    }
}
