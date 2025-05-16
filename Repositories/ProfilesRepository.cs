using tenis_pro_back.Models;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Repositories
{
	public class ProfilesRepository: IProfile
    {
		private readonly IMongoCollection<Profile> _profiles;

		public ProfilesRepository(IMongoDatabase database)
		{
            _profiles = database.GetCollection<Profile>("profiles");
		}

		public async Task<IEnumerable<Profile>> GetAll()
		{
			return await _profiles.Find(u => true).ToListAsync();
		}

		public async Task<Profile> GetById(string id)
		{
			return await _profiles.Find(u => u.Id == id).FirstOrDefaultAsync();
		}

        public async Task<Profile?> GetByType(Profile.ProfileType type)
        {
            return await _profiles.Find(u => u.Type == type).FirstOrDefaultAsync();
        }

        public async Task<Profile> Post(Profile profile)
        {
			await _profiles.InsertOneAsync(profile);
			return profile;

        }

		public async Task Put(string id, Profile profile)
		{
			await _profiles.ReplaceOneAsync(u => u.Id == id, profile);
		}

		public async Task Delete(string id)
		{
			await _profiles.DeleteOneAsync(u => u.Id == id);
		}
	}
}
