using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tenis_pro_back.Models.Dto
{
    public class RegisterUserDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Phone1 { get; set; }
        public required string Email { get; set; }
        public string? CategoryId { get; set; } //id categoria actual
        public string? Image { get; set; }
        public string? BirthDate { get; set; }
        public required string Password { get; set; }
    }
}
