using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Repositories
{
    public class UserTokenRepository: IUserToken
    {
        private readonly IMongoCollection<UserToken> _userTokens;

        public UserTokenRepository(IMongoDatabase database)
        {
            _userTokens = database.GetCollection<UserToken>("UserTokens");
        }

        public async Task CreateAsync(UserToken token)
        {
            await _userTokens.InsertOneAsync(token);
        }

        public async Task<UserToken> GetTokenAsync(Guid token)
        {
            return await _userTokens
                .Find(t => t.Token == token)
                .FirstOrDefaultAsync();
        }

        public async Task MarkTokenAsUsedAsync(Guid token)
        {
            var update = Builders<UserToken>.Update.Set(t => t.Used, true);
            await _userTokens.UpdateOneAsync(t => t.Token == token, update);
        }

        public async Task DeleteTokenAsync(Guid token)
        { 
            await _userTokens.DeleteOneAsync(u => u.Token == token);
        }
    }
}
