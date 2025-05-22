using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Helpers;
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
        [AllowAnonymous]
        [HttpGet]
		public async Task<ActionResult<List<Category>>> Get()
		{
			try
			{
				var categories = await _categoryRepository.GetAll();
				return Ok(categories.OrderBy(o => o.Description));
			}
			catch (Exception ex)
			{
				HandleErrorHelper.LogError(ex);
				return BadRequest(ex.Message);

			}
        }

        // GET: api/categories/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
		public async Task<ActionResult<Category>> GetCategoryById(string id)
		{
			try
			{
				var category = await _categoryRepository.GetById(id);
				if (category == null) return NotFound();
				return Ok(category);
			}
			catch (Exception ex)
			{
				HandleErrorHelper.LogError(ex);
				return BadRequest(ex.Message);

			}
        }

        // POST: api/categories
        [Authorize]
        [HttpPost]
		public async Task<ActionResult> Create(Category category)
		{
			try
			{
				await _categoryRepository.Post(category);
				return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // PUT: api/categories/{id}
        [Authorize]
        [HttpPut("{id}")]
		public async Task<ActionResult> Update(string id, Category category)
		{
			try
			{
				var existingCategory = await _categoryRepository.GetById(id);
				if (existingCategory == null) return NotFound();

				await _categoryRepository.Put(id, category);
				return NoContent();
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

        // DELETE: api/categories/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
			try
			{
				var existingCategory = await _categoryRepository.GetById(id);
				if (existingCategory == null) return NotFound();

				await _categoryRepository.Delete(id);
				return NoContent();
			}
            catch (Exception ex)
            {
                HandleErrorHelper.LogError(ex);
                return BadRequest(ex.Message);

            }
        }

    }
}