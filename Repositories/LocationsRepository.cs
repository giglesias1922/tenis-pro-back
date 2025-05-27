using tenis_pro_back.Models;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Repositories
{
	public class LocationsRepository : ILocation
    {
		private readonly IMongoCollection<Location> _locations;
        private readonly IMongoCollection<Tournament> _tournaments;

        public LocationsRepository(IMongoDatabase database)
		{
			_locations = database.GetCollection<Location>("Locations");
            _tournaments = database.GetCollection<Tournament>("Tournaments");
        }

		public async Task<List<Location>> GetAll()
		{
			return await _locations.Find(location => true).ToListAsync();
		}

		public async Task<Location> GetById(string id)
		{
			return await _locations.Find(location => location.Id == id).FirstOrDefaultAsync();
		}

		public async Task Post(Location location)
		{
			await _locations.InsertOneAsync(location);
		}

		public async Task<bool> Put(string id, Location location)
		{
			var result = await _locations.ReplaceOneAsync(l => l.Id == id, location);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> Delete(string id)
		{
            var tournamentCount = await _tournaments.CountDocumentsAsync(t => t.CategoryId == id);

			if (tournamentCount > 0)
			{
				throw new Exception("No se puede eliminar la sede porque hay torneos asociados a ella.");
			}

            var result = await _locations.DeleteOneAsync(location => location.Id == id);
			return result.IsAcknowledged && result.DeletedCount > 0;
		}
	}
}
