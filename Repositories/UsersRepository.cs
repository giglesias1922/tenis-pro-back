using tenis_pro_back.Models;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Repositories
{
	public class UsersRepository :IUser
	{
		private readonly IMongoCollection<User> _users;
		
        public UsersRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("users");
        }

        public async Task<IEnumerable<User>> GetAll()
		{
			return await _users.Find(u => true).ToListAsync();
		}

        public async Task<IEnumerable<User>> GetByCategory(string categoryId)
        {
            return await _users.Find(u=>u.CategoryId == categoryId).ToListAsync();
        }

        public async Task<User> GetById(string id)
		{
			return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
		}


        public async Task Post(User user)
		{
			await _users.InsertOneAsync(user);
		}

		public async Task Put(string id, User user)
		{
			await _users.ReplaceOneAsync(u => u.Id == id, user);
		}

		public async Task Delete(string id)
		{
            await _users.DeleteOneAsync(u => u.Id == id);
		}
	}
}
