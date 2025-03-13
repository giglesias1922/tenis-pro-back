using tenis_pro_back.Models;
using MongoDB.Driver;

namespace tenis_pro_back.Repositories
{
	public class FunctionalitiesRepository
    {
		private readonly IMongoCollection<Functionality> _Functionalities;
		
        public FunctionalitiesRepository(IMongoDatabase database)
        {
            _Functionalities = database.GetCollection<Functionality>("functionalities");
        }

        public async Task<List<Functionality>> GetAllAsync()
		{
			return await _Functionalities.Find(_ => true).ToListAsync();
		}

		public async Task<Functionality> GetByIdAsync(string id)
		{
			return await _Functionalities.Find(c => c.Id == id).FirstOrDefaultAsync();
		}

		public async Task CreateAsync(Functionality Functionality)
		{
			await _Functionalities.InsertOneAsync(Functionality);
		}

		public async Task UpdateAsync(string id, Functionality Functionality)
		{
			await _Functionalities.ReplaceOneAsync(c => c.Id == id, Functionality);
		}

		public async Task DeleteAsync(string id)
		{
			await _Functionalities.DeleteOneAsync(c => c.Id == id);
		}
	}
}