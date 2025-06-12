namespace tenis_pro_back.Models.Dto
{
    public class ResetPasswordDto
    {
        public required string Email { get; set; }
        public required string RedirectUrl { get; set; }
    }
}
