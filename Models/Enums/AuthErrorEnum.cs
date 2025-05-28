namespace tenis_pro_back.Models.Enums
{
    public enum AuthErrorEnum: Int16
    {
        UserNotFound = 1,
        InvalidCredentials =2,
        UserDisabled=3,
        AccountLocked=4,
        Unauthorized
    }
}
