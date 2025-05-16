namespace tenis_pro_back.Models
{
    public class UserActivationToken
    {
        public required string UserId { get; set; }
        public required string Token { get; set; } 
        public required DateTime Expiration { get; set; }

        public User? User { get; set; }
    }
}
