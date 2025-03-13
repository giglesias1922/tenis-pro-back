using tenis_pro_back.Models;
using tenis_pro_back.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace tenis_pro_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly UsersRepository _userRepository;

		public UsersController(UsersRepository userRepository)
		{
			_userRepository = userRepository;
		}

		// GET: api/users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetAll()
		{
			var users = await _userRepository.GetAllUsersAsync();
			return Ok(users);
		}

		// GET: api/users/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetById(string id)
		{
			var user = await _userRepository.GetUserByIdAsync(id);

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
			await _userRepository.CreateUserAsync(user);
			return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
		}

		// PUT: api/users/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, User user)
		{
			var existingUser = await _userRepository.GetUserByIdAsync(id);

			if (existingUser == null)
			{
				return NotFound();
			}

			await _userRepository.UpdateUserAsync(id, user);
			return NoContent();
		}

		// DELETE: api/users/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userRepository.GetUserByIdAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			await _userRepository.DeleteUserAsync(id);
			return NoContent();
		}
	}
}
