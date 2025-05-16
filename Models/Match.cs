using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models
{
    public class Match
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tournamentid")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string TournamentId { get; set; }

        [BsonElement("registrations")]
        public required List<string> registrations { get; set; } 

        [BsonElement("status")]
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        [BsonElement("history")]
        public List<MatchHistory> History { get; set; } = new();

        [BsonElement("result")]
        public MatchResult? Result { get; set; } // Ej: "6-3, 4-6, 10-7"

        [BsonElement("createdby")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CreatedBy { get; set; }

        [BsonElement("createdate")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public enum MatchStatus
        {
            Scheduled=0,
            InProgress=1,
            Completed=2,
            Suspended=3,
            Cancelled=4
        }


        public class MatchHistory
        {
            [BsonElement("date")]
            public DateTime Date { get; set; } = DateTime.UtcNow;
            [BsonElement("status")]
            public MatchStatus Status { get; set; }
            [BsonElement("notes")]
            public string? Notes { get; set; }
        }

        public class MatchResult
        {
            [BsonElement("winner")]
            [BsonRepresentation(BsonType.ObjectId)]
            public required string Winner { get; set; }

            [BsonElement("sets")]
            public required List<SetResult> Sets { get; set; } = new();
        }

        public class SetResult
        {
            [BsonElement("playerA_games")]
            public int PlayerAGames { get; set; }

            [BsonElement("playerB_games")]
            public int PlayerBGames { get; set; }

            [BsonElement("tiebreak")]
            public string? Tiebreak { get; set; } // Ej: "7-5", null si no hubo

            [BsonElement("winnerSet")]
            [BsonRepresentation(BsonType.ObjectId)]
            public required string WinnerSet { get; set; }
        }

    }
}
