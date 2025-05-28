namespace tenis_pro_back.Models.Response
{
    public class LoginResponse
    {
        public string? Token { get; set; }   
        public int ResponseCode { get; set; }
        public string? ResponseMessage { get; set; }
    }
}
