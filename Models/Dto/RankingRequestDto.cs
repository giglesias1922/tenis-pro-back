namespace tenis_pro_back.Models.Dto
{
    public class RankingRequestDto
    {
        public required string CategoryId { get; set; }
        public Models.Enums.TournamentTypeEnum TournamentType { get; set; } // 1 o 2
        public int Year { get; set; }
    }
}
