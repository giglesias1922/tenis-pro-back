﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using static tenis_pro_back.Models.Match;

namespace tenis_pro_back.Models.Dto
{
    public class CreateMatchDto
    {
        public required string TournamentId { get; set; }

        public MatchStatus Status { get; set; } 

        public List<MatchHistory> History { get; set; } = new();

        public string? CreatedBy { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string? Participant1Id { get; set; }
        public string? Participant2Id { get; set; }
    }
}
