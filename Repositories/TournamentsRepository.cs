// TournamentRepository.cs
using tenis_pro_back.Models;
using MongoDB.Driver;

namespace tenis_pro_back.Repositories
{
	public class TournamentsRepository
	{
		private readonly IMongoCollection<Tournament> _tournaments;
		private readonly IMongoCollection<Category> _categories;
		private readonly IMongoCollection<Location> _locations;

		public TournamentsRepository(IMongoDatabase database)
		{
			_tournaments = database.GetCollection<Tournament>("tournaments");
			_categories = database.GetCollection<Category>("categories");
			_locations = database.GetCollection<Location>("locations");
		}

		public async Task<IEnumerable<Tournament>> GetAllTournamentsAsync()
		{
			var tournaments = await _tournaments.Find(t => true).ToListAsync();
			var categories = await _categories.Find(t => true).ToListAsync();
			var locations = await _locations.Find(t => true).ToListAsync();

			
			// Para cada torneo, obtener las categorías y locaciones relacionadas
			foreach (var tournament in tournaments)
			{
				// Asignar los datos obtenidos al torneo
				tournament.CategoryDesc = categories.Find(f => f.Id == tournament.CategoryId)?.Description ?? null;
				tournament.LocationName = locations.Find(f => f.Id == tournament.LocationId)?.name ?? null;
			}

			return tournaments;
		}


        public async Task<long> CountTournamentsByCategoryIdAsync(string categoryId)
        {
            return await _tournaments.CountDocumentsAsync(t => t.CategoryId == categoryId);
        }

        public async Task<long> CountTournamentsByLocationIdAsync(string locationId)
        {
            return await _tournaments.CountDocumentsAsync(t => t.LocationId == locationId);
        }

        public async Task<Tournament> GetTournamentByIdAsync(string id)
		{
			return await _tournaments.Find(t => t.Id == id).FirstOrDefaultAsync();
		}

		public async Task CreateTournamentAsync(Tournament tournament)
		{
			await _tournaments.InsertOneAsync(tournament);
		}

		public async Task UpdateTournamentAsync(string id, Tournament tournament)
		{
			await _tournaments.ReplaceOneAsync(t => t.Id == id, tournament);
		}

		public async Task DeleteTournamentAsync(string id)
		{
			await _tournaments.DeleteOneAsync(t => t.Id == id);
		}
	}
}
