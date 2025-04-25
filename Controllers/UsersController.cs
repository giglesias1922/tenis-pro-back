using Microsoft.AspNetCore.Mvc;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUser _userRepository;

		public UsersController(IUser userRepository)
		{
			_userRepository = userRepository;
		}

        // GET: api/users
        [HttpGet]
		[Route("Category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetByCategory(string categoryId)
        {
            var users = await _userRepository.GetByCategory(categoryId);
            return Ok(users);
        }

        // GET: api/users
        [HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetAll()
		{
			var users = await _userRepository.GetAll();
			return Ok(users);
		}

		// GET: api/users/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetById(string id)
		{
			var user = await _userRepository.GetById(id);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		// POST: api/users
		[HttpPost]
		public async Task<ActionResult> Create(User user)
		{
			user.Id = null; // Deja que MongoDB genere el Id automáticamente
			await _userRepository.Post(user);
			return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
		}

		// PUT: api/users/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, User user)
		{
			try
			{
				var existingUser = await _userRepository.GetById(id);

				if (existingUser == null)
				{
					return NotFound();
				}

				await _userRepository.Put(id, user);
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE: api/users/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userRepository.GetById(id);

			if (user == null)
			{
				return NotFound();
			}

			await _userRepository.Delete(id);
			return NoContent();
		}
	}
}
