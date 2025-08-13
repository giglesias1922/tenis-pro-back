using MongoDB.Bson;

namespace tenis_pro_back.Models.Dto
{
    public class TournamentZone
{
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Match> Matches { get; set; }
    }
}
