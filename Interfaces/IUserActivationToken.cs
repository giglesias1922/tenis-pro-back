using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IUserActivationToken
    {
        Task Post(UserActivationToken obj);
        
        Task<UserActivationToken?> GetByToken(string token);
        Task Delete(string token);
        Task<UserActivationToken?> GetByUserid(string userId);
    }
}
