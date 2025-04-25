using tenis_pro_back.Models;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Repositories
{
	public class FunctionalitiesRepository: IFunctionality
    {
		private readonly IMongoCollection<Functionality> _Functionalities;
		
        public FunctionalitiesRepository(IMongoDatabase database)
        {
            _Functionalities = database.GetCollection<Functionality>("functionalities");
        }

        public async Task<List<Functionality>> GetAll()
		{
			return await _Functionalities.Find(_ => true).ToListAsync();
		}

		public async Task<Functionality> GetById(string id)
		{
			return await _Functionalities.Find(c => c.Id == id).FirstOrDefaultAsync();
		}

		public async Task Post(Functionality Functionality)
		{
			await _Functionalities.InsertOneAsync(Functionality);
		}

		public async Task Put(string id, Functionality Functionality)
		{
			await _Functionalities.ReplaceOneAsync(c => c.Id == id, Functionality);
		}

		public async Task Delete(string id)
		{
			await _Functionalities.DeleteOneAsync(c => c.Id == id);
		}
	}
}