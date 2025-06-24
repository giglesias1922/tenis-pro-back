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
		public DateTime? CloseDate { get; set; } //cierre de inscripcion

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

        [BsonElement("tournamentype")]
        public required Models.Enums.TournamentTypeEnum TournamentType { get; set; }  // single /doble

        [BsonElement("status")]
		public required Models.Enums.TournamentStatusEnum Status { get; set; }

        [BsonElement("image")]
        public string? Image { get; set; }

        [BsonElement("groupSize")]
        public int PlayersPerZone { get; set; } = 4; // jugadores por zona

        [BsonElement("classifyTop")]
        public int QualifiersPerZone { get; set; } = 2; // cuántos clasifican por zona

        [BsonElement("includePlata")]
        public bool IncludePlata { get; set; } = false;

        [BsonElement("participants")]
        public List<Participant> Participants { get; set; } = new();

        [BsonElement("zones")]
        public List<Zone> Zones { get; set; } = new();

        [BsonElement("mainBracket")]
        public Bracket? MainBracket { get; set; }

        [BsonElement("silverCupBracket")]
        public Bracket? SilverCupBracket { get; set; }
    }

    public class Participant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("playerIds")]
        public required List<string> PlayerIds { get; set; } // 1 for singles, 2 for doubles

        [BsonElement("displayName")]
        public required string DisplayName { get; set; }

        [BsonElement("ranking")]
        public int Ranking { get; set; } = 999; // Default ranking, will be updated
    }

    public class Zone
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("participants")]
        public List<string> ParticipantIds { get; set; } = new();
    }

    public class Bracket
    {
        [BsonElement("rounds")]
        public List<Round> Rounds { get; set; } = new();
    }

    public class Round
    {
        [BsonElement("name")]
        public required string Name { get; set; } // e.g., "Quarter-finals"

        [BsonElement("matches")]
        public List<string> MatchIds { get; set; } = new();
    }
}
