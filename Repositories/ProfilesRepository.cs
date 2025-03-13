using tenis_pro_back.Models;
using MongoDB.Driver;

namespace tenis_pro_back.Repositories
{
	public class ProfilesRepository
    {
		private readonly IMongoCollection<Profile> _profiles;

		public ProfilesRepository(IMongoDatabase database)
		{
            _profiles = database.GetCollection<Profile>("profiles");
		}

		public async Task<IEnumerable<Profile>> GetAllAsync()
		{
			return await _profiles.Find(u => true).ToListAsync();
		}

		public async Task<Profile> GetByIdAsync(string id)
		{
			return await _profiles.Find(u => u.Id == id).FirstOrDefaultAsync();
		}

		public async Task CreateAsync(Profile profile)
        {
			await _profiles.InsertOneAsync(profile);
		}

		public async Task UpdateAsync(string id, Profile profile)
		{
			await _profiles.ReplaceOneAsync(u => u.Id == id, profile);
		}

		public async Task DeleteAsync(string id)
		{
			await _profiles.DeleteOneAsync(u => u.Id == id);
		}
	}
}
