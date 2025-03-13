using tenis_pro_back.Models;
using MongoDB.Driver;

namespace tenis_pro_back.Repositories
{
	public class UsersRepository 
	{
		private readonly IMongoCollection<User> _users;

		public UsersRepository(IMongoDatabase database)
		{
			_users = database.GetCollection<User>("users");
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _users.Find(u => true).ToListAsync();
		}

		public async Task<User> GetUserByIdAsync(string id)
		{
			return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
		}


        public async Task CreateUserAsync(User user)
		{
			await _users.InsertOneAsync(user);
		}

		public async Task UpdateUserAsync(string id, User user)
		{
			await _users.ReplaceOneAsync(u => u.Id == id, user);
		}

		public async Task DeleteUserAsync(string id)
		{
			await _users.DeleteOneAsync(u => u.Id == id);
		}
	}
}
