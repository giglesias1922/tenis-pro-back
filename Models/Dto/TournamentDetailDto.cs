using MongoDB.Bson.Serialization.Attributes;

namespace tenis_pro_back.Models.Dto
{
    public class TournamentDetailDto
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public DateTime? CloseDate { get; set; } //cierre de inscripcion
        public DateTime? InitialDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TournamentStatus? Status { get; set; }
        public string? CategoryDescription { get; set; }
        public string? LocationDescription { get; set; }
        public string? CategoryId { get; set; }
        public string? LocationId { get; set; }
        public string? StatusName { get; set; }
        public TournamentType? TournamentType { get; set; }
        public string ? TournamentTypeDescription { get; set; }
    }
}
