using tenis_pro_back.Models;
using MongoDB.Driver;

namespace tenis_pro_back.Repositories
{
	public class LocationsRepository 
	{
		private readonly IMongoCollection<Location> _locations;

		public LocationsRepository(IMongoDatabase database)
		{
			_locations = database.GetCollection<Location>("locations");
		}

		public async Task<List<Location>> GetLocationsAsync()
		{
			return await _locations.Find(location => true).ToListAsync();
		}

		public async Task<Location> GetLocationByIdAsync(string id)
		{
			return await _locations.Find(location => location.Id == id).FirstOrDefaultAsync();
		}

		public async Task CreateLocationAsync(Location location)
		{
			await _locations.InsertOneAsync(location);
		}

		public async Task<bool> UpdateLocationAsync(string id, Location location)
		{
			var result = await _locations.ReplaceOneAsync(l => l.Id == id, location);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> DeleteLocationAsync(string id)
		{
			var result = await _locations.DeleteOneAsync(location => location.Id == id);
			return result.IsAcknowledged && result.DeletedCount > 0;
		}
	}
}
