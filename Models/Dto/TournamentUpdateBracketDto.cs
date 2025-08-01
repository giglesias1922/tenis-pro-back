namespace tenis_pro_back.Models.Dto
{
    public class TournamentUpdateBracketDto
    {

        public string? Id { get; set; }
        public Models.Enums.TournamentStatusEnum Status { get; set; }
        public Bracket? DrawBracket { get; set; }

    }
}
