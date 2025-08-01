namespace tenis_pro_back.Models.Dto
{
    public class MatchUpdateDto
    {
        public string MatchId { get; set; } = default!; // ID del match a actualizar

        public string Winner { get; set; } = default!; // ID del participante ganador

        public int? Points { get; set; } // Opcional: puntos obtenidos

        public List<UpdateSetDto> Sets { get; set; } = new(); // Detalle de sets
    }

    public class UpdateSetDto
    {
        public int PlayerA_games { get; set; }
        public int PlayerB_games { get; set; }
        public string? Tiebreak { get; set; }
        public string WinnerSet { get; set; } = default!; // ID del jugador ganador del set
    }
}
