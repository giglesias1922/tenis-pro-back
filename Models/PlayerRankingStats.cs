namespace tenis_pro_back.Models
{
    public class PlayerRankingStats
    {
        public PlayerRankingStats(string playerId)
        {
            PlayerId = playerId;
        }

        public string PlayerId { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int TotalPoints { get; set; }
    }
}
