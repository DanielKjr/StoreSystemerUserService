using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers
{
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		[Route("new")]
		public async Task AddUser([FromBody] UserDTO user)
		{
			await _userService.CreateUser(user);
		}



		[HttpGet("byName/{name}")]
		public async Task<User> GetUser(string name)
		{
			return await _userService.GetUserByName(name);
		}
		[HttpGet]
		[Route("all")]
		public async Task<List<User>> GetUsers()
		{
			return await _userService.GetAllUsers();
		}


		[HttpPost]
		[Route("login")]
		public async Task<string> Login([FromBody] UserDTO user)
		{
			return await Task.FromResult(await _userService.Login(user));
		}

		[HttpPatch("addItems/{id}")]
		public Task AddItems(Guid id, [FromBody] List<Item> stuff)
		{
			return _userService.AddItemsToUser(id, stuff);
		}


		[HttpDelete("delete/{id}")]
		public async Task DeleteUser(Guid id)
		{
			await _userService.DeleteUser(id);
		}

		[HttpDelete("delete/all")]
		public async Task DeleteAll()
		{
			await _userService.DeleteAll();
		}


		//debug method
		[HttpPost]
		[Route("addto{id}")]
		public async Task AddUser(Guid id)
		{
			await _userService.GetUserById(id);

			var items = new List<Item>
			{
				new Item {Name = "Item1", Price = 100},
				new Item {Name = "Item2", Price = 200},
				new Item {Name = "Item3", Price = 300}
			};
			await _userService.AddItemsToUser(id, items);
		}
	}
}
