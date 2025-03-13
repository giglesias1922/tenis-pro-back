using tenis_pro_back.Models;
using MongoDB.Driver;

namespace tenis_pro_back.Repositories
{
	public class CategoriesRepository
	{
		private readonly IMongoCollection<Category> _categories;
		
        public CategoriesRepository(IMongoDatabase database)
        {
            _categories = database.GetCollection<Category>("categories");
        }

        public async Task<List<Category>> GetCategoriesAsync()
		{
			return await _categories.Find(_ => true).ToListAsync();
		}

		public async Task<Category> GetCategoryByIdAsync(string id)
		{
			return await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();
		}

		public async Task CreateCategoryAsync(Category category)
		{
			await _categories.InsertOneAsync(category);
		}

		public async Task UpdateCategoryAsync(string id, Category category)
		{
			await _categories.ReplaceOneAsync(c => c.Id == id, category);
		}

		public async Task DeleteCategoryAsync(string id)
		{
			await _categories.DeleteOneAsync(c => c.Id == id);
		}
	}
}