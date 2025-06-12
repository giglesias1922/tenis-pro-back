using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IUserToken
    {
        Task CreateAsync(UserToken userToken);
        Task<UserToken> GetTokenAsync(Guid token);
        Task MarkTokenAsUsedAsync(Guid token);
         Task DeleteTokenAsync(Guid token);
    }
}
