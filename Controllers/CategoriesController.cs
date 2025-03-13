using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tenis_pro_back.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController : ControllerBase
	{
		private readonly CategoriesRepository _categoryRepository;
		private readonly TournamentsRepository _tournamentRepository;

        public CategoriesController(CategoriesRepository categoryRepository, TournamentsRepository tournamentRepository)
        {
            _categoryRepository = categoryRepository;
            _tournamentRepository = tournamentRepository;
        }

        // GET: api/categories
        [HttpGet]
		public async Task<ActionResult<List<Category>>> Get()
		{
			var categories = await _categoryRepository.GetCategoriesAsync();
			return Ok(categories.OrderBy(o=>o.Description));
		}

		// GET: api/categories/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Category>> Get(string id)
		{
			var category = await _categoryRepository.GetCategoryByIdAsync(id);
			if (category == null) return NotFound();
			return Ok(category);
		}

		// POST: api/categories
		[HttpPost]
		public async Task<ActionResult> Create(Category category)
		{
			await _categoryRepository.CreateCategoryAsync(category);
			return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
		}

		// PUT: api/categories/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> Update(string id, Category category)
		{
			var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
			if (existingCategory == null) return NotFound();

			await _categoryRepository.UpdateCategoryAsync(id, category);
			return NoContent();
		}

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null) return NotFound();

            // Verificar si hay torneos asociados a esta categoría
            long tournamentCount = await _tournamentRepository.CountTournamentsByCategoryIdAsync(id);
            if (tournamentCount > 0)
            {
                return BadRequest("No se puede eliminar la categoría porque hay torneos asociados a ella.");
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            return NoContent();
        }

    }
}