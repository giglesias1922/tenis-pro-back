using tenis_pro_back.Models;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;

namespace tenis_pro_back.Repositories
{
	public class CategoriesRepository:ICategory
	{
		private readonly IMongoCollection<Category> _categories;
        private readonly IMongoCollection<Tournament> _tournaments;

        public CategoriesRepository(IMongoDatabase database)
        {
            _categories = database.GetCollection<Category>("Categories");
            _tournaments = database.GetCollection<Tournament>("Tournaments");
        }

        public async Task<List<Category>> GetAll()
		{
			return await _categories.Find(_ => true).ToListAsync();
		}

		public async Task<Category> GetById(string id)
		{
			return await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();
		}

        public async Task<List<Category>> GetEnabledCategories()
        {
            return await _categories.Find(w => w.Active == true).ToListAsync();
        }

        public async Task Post(Category category)
		{
			await _categories.InsertOneAsync(category);
		}

		public async Task Put(string id, Category category)
		{
			await _categories.ReplaceOneAsync(c => c.Id == id, category);
		}

		public async Task Delete(string id)
		{
            var tournamentCount = await _tournaments.CountDocumentsAsync(t => t.CategoryId == id);

			if (tournamentCount > 0)
			{
				//Si hay torneos con esa categoría, no la elimina, la desactiva
				Category obj = await GetById(id);

				if (obj == null)
					throw new Exception("Category not found.");

				obj.Active = false;

				await Put(id, obj);

			}
			else
			{
				await _categories.DeleteOneAsync(c => c.Id == id);
			}
		}
	}
}