using UserService.DTOs;
using UserService.Models;

namespace UserService.Interfaces
{
	public interface IUserService
	{
		Task CreateUser(UserDTO user);
		Task<string> Login(UserDTO user);
		Task<bool> AddItemsToUser(Guid id, List<Item> stuff);
		Task<bool> AddItemToUser(Guid id, Item stuff);
		Task<bool> UpdateUser(User user);
		Task<User> GetUserByName(string username);
		Task<User> GetUserById(Guid id);
		Task<List<User>> GetAllUsers();
		Task DeleteUser(Guid id);
		Task DeleteAll();
	}
}
