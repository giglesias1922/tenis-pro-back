using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Repositories
{
    public class UserActivationTokenRepository:  IUserActivationToken
    {
        private readonly IMongoCollection<UserActivationToken> _userActivationToken;

        public UserActivationTokenRepository(IMongoCollection<UserActivationToken> userActivationToken)
        {
            _userActivationToken = userActivationToken;
        }

        public async Task<UserActivationToken?> GetByToken(string token)
        {
            return await _userActivationToken.Find(c => c.Token == token).FirstOrDefaultAsync();
        }

        public async Task Post(UserActivationToken userActivationToken)
        {
            await _userActivationToken.InsertOneAsync(userActivationToken);
        }

        public async Task Delete(string token)
        {
            await _userActivationToken.DeleteOneAsync(c => c.Token == token);
        }
    }
}
