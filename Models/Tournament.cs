using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace tenis_pro_back.Models
{
	public class Tournament
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonElement("description")]
		public required string Description { get; set; }

		[BsonElement("closedate")] 
		public DateTime? closeDate { get; set; } //cierre de inscripcion

		[BsonElement("initialdate")]
		public DateTime? InitialDate { get; set; }

		[BsonElement("enddate")]
		public DateTime? EndDate { get; set; }

		[BsonElement("locationid")]
		[BsonRepresentation(BsonType.ObjectId)]
		public required string LocationId { get; set; }  // IDs de las locaciones

		[BsonElement("categoryid")]
		[BsonRepresentation(BsonType.ObjectId)]
		public required string CategoryId { get; set; }  // IDs de las categorías

		[BsonElement("status")]
		public required TournamentStatus? Status { get; set; } = TournamentStatus.Pending;

		public string? CategoryDesc { get; set; }
		public string? LocationName { get; set; } 

	}

	public enum TournamentStatus
	{
		Pending = 0,
		Initiated = 1,
		Finalized = 2
	}
}
