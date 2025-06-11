namespace tenis_pro_back.Models.Dto
{
    public class ChangePasswordDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
