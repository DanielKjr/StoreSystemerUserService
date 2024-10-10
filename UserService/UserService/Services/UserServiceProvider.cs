using EF_InteractionFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserService.Context;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services
{
	public class UserServiceProvider : IUserService
	{

		private readonly IAsyncRepository<UserContext> _repository;
		private readonly IJwtProvider _jwtProvider;
		private readonly IRepositoryPatch<UserContext> _repositoryPatch;

		public UserServiceProvider(IAsyncRepository<UserContext> repository, IJwtProvider jwtProvider, IRepositoryPatch<UserContext> repositoryPatch)
		{
			_repository = repository;
			_jwtProvider = jwtProvider;
			_repositoryPatch = repositoryPatch;
		}

		public async Task CreateUser(UserDTO user)
		{
			var newUser = new User()
			{
				Username = user.Username,
				Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
			};
			var existingUser = await _repository.GetItem<User>(x => x.Where(x => x.Username == newUser.Username))!;

			if (existingUser == null)
				await _repository.AddItem(newUser);
		}


		public async Task<string> Login(UserDTO user)
		{
			var existingUser = await _repository.GetItem<User>(x => x.Where(x => x.Username == user.Username))!;
			if (existingUser != null)
			{
				if (BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
					return _jwtProvider.CreateToken(existingUser);
				else
					return "Username or password is incorrect.";
			}
			else
				return "Username or password is incorrect.";

		}


		public async Task<bool> AddItemsToUser(Guid id, List<Item> stuff)
		{
			bool succes = _repositoryPatch.AddItemsToUser<User, Item>(q => q.Where(x => x.Id == id).Include(i => i.Inventory).Include(i => i.Inventory.Items), null!, stuff).IsCompleted;
			return await Task.FromResult(succes);
		}

		public async Task<bool> AddItemToUser(Guid id, Item item)
		{
			bool succes = _repositoryPatch.AddItemsToUser<User, Item>(q => q.Where(x => x.Id == id).Include(i => i.Inventory.Items), item, null!).IsCompleted;
			return await Task.FromResult(succes);
		}

		public async Task<bool> UpdateUser(User user)
		{
			return await Task.FromResult(_repositoryPatch.UpdateUser(user).IsCompletedSuccessfully);
		}

		public async Task<User> GetUserByName(string username)
		{
			return await _repository.GetItem<User>(q => q.Where(x => x.Username == username).Include(x => x.Inventory).Include(e => e.Inventory.Items));
		}

		public async Task<User> GetUserById(Guid id)
		{
			return await _repository.GetItem<User>(q => q.Where(x => x.Id == id));
		}
		public async Task<List<User>> GetAllUsers()
		{
			return await _repository.GetAllItems<User>(q => q.Include(x => x.Inventory).Include(e => e.Inventory.Items));
		}


		public async Task DeleteUser(Guid id)
		{

			await _repository.RemoveItem<User>(x => x.Id == id);
		}

		public async Task DeleteAll()
		{
			await _repository.RemoveItems<User>(x => x.Where(z => z.Username != string.Empty));
		}


	}
}
