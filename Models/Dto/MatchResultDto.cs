using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace tenis_pro_back.Models.Dto
{
    public class MatchResultDto
    {
        [BsonElement("winner")]
        public required string Winner { get; set; }

        [BsonElement("sets")]
        public required List<SetResultDto> Sets { get; set; }
        public Int32? Points { get; set; }
    }

    public class SetResultDto
    {
        [BsonElement("playerA_games")]
        public int PlayerA_games { get; set; }

        [BsonElement("playerB_games")]
        public int PlayerB_games { get; set; }
        [BsonElement("tiebreak")]
        public string? Tiebreak { get; set; }

        [BsonElement("winnerSet")]
        public string? WinnerSet { get; set; }
    }
}
