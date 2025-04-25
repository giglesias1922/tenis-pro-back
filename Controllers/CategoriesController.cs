using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategory _categoryRepository;
		private readonly ITournament _tournamentRepository;

        public CategoriesController(ICategory categoryRepository, ITournament tournamentRepository)
        {
            _categoryRepository = categoryRepository;
            _tournamentRepository = tournamentRepository;
        }

        // GET: api/categories
        [HttpGet]
		public async Task<ActionResult<List<Category>>> Get()
		{
			var categories = await _categoryRepository.GetAll();
			return Ok(categories.OrderBy(o=>o.Description));
		}

		// GET: api/categories/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Category>> GetCategoryById(string id)
		{
			var category = await _categoryRepository.GetById(id);
			if (category == null) return NotFound();
			return Ok(category);
		}

		// POST: api/categories
		[HttpPost]
		public async Task<ActionResult> Create(Category category)
		{
			await _categoryRepository.Post(category);
			return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
		}

		// PUT: api/categories/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> Update(string id, Category category)
		{
			var existingCategory = await _categoryRepository.GetById(id);
			if (existingCategory == null) return NotFound();

			await _categoryRepository.Put(id, category);
			return NoContent();
		}

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingCategory = await _categoryRepository.GetById(id);
            if (existingCategory == null) return NotFound();

            await _categoryRepository.Delete(id);
            return NoContent();
        }

    }
}