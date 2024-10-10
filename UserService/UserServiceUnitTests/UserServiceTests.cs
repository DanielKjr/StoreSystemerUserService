using EF_InteractionFrameworkCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Context;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Models;
using UserService.Services;
using UserServiceUnitTests.Utility;
using static NUnit.Framework.Assert;

namespace UserServiceUnitTests
{
	internal class UserServiceTests
	{
		private ServiceProvider _serviceProvider;
		private User user;
		private UserDTO userDto;
		private List<UserDTO> usersToAdd;
		private IUserService _userService;


		public UserServiceTests()
		{
			var secretFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../../secret.txt"));

			// Set the environment variable
			Environment.SetEnvironmentVariable("secretPath", secretFilePath);
			user = new User() { Username = "something", Password = "password" };
			user.Inventory.Items.Add(new Item() { Name = "Some item", Price = 50 });
			userDto = new UserDTO() { Username = user.Username, Password = user.Password };
			usersToAdd = new List<UserDTO>();

			for (int i = 0; i < 10; i++)
			{
				usersToAdd.Add(new UserDTO() { Username = $"user{i}", Password = $"password{i}" });
			}
		}

		[SetUp]
		public void Setup()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<UserContext>();
			var _repository = _serviceProvider.GetRequiredService<IAsyncRepository<UserContext>>();
			_serviceProvider.GetRequiredService<UserContext>().Database.EnsureCreated();
			var _jwtProvider = new JwtProvider();
			var _repositoryPatch = _serviceProvider.GetRequiredService<IRepositoryPatch<UserContext>>();
			_userService = new UserServiceProvider(_repository, _jwtProvider, _repositoryPatch);


		}


		[TearDownAttribute]
		public async Task Dispose()
		{
			await _serviceProvider.DisposeAsync();
		}

		[Test]
		public async Task CanCreateUser()
		{
			await _userService.CreateUser(userDto);
			var retrievedUser = await _userService.GetUserByName(user.Username);
			That(retrievedUser != null);
			That(retrievedUser!.Username == user.Username);
		}

		[Test]
		public async Task CantCreateUserWithSameName()
		{
			await _userService.CreateUser(userDto);
			await _userService.CreateUser(userDto);
			var users = await _userService.GetAllUsers();
			That(users.Count() == 1);
		}



		[Test]
		public async Task CanLogin()
		{

			await _userService.CreateUser(userDto);
			string returnvalue = await _userService.Login(userDto);
			var tokenHandler = new JwtSecurityTokenHandler();
			var canread = tokenHandler.CanReadToken(returnvalue);
			That(returnvalue != "Username or password is incorrect.");
			That(returnvalue.Length >= 100);
			That(canread);
		}

		[Test]
		public async Task LoginWIthWrongNameGivesIncorrectMessage()
		{
			await _userService.CreateUser(userDto);

			var wrongUser = new UserDTO() { Username = "wrong", Password = user.Password };
			string returnvalue = await _userService.Login(wrongUser);
			var tokenHandler = new JwtSecurityTokenHandler();
			var canread = tokenHandler.CanReadToken(returnvalue);

			That(!canread);
		}

		[Test]
		public async Task LoginWIthWrongPasswordGivesIncorrectMessage()
		{
			await _userService.CreateUser(userDto);
			var wrongUser = new UserDTO() { Username = user.Username, Password = "wrongvalue" };
			string returnvalue = await _userService.Login(wrongUser);
			string correctValue = await _userService.Login(userDto);
			var tokenHandler = new JwtSecurityTokenHandler();
			var canReadWrongUser = tokenHandler.CanReadToken(returnvalue);
			var canReadRightUser = tokenHandler.CanReadToken(correctValue);
			That(!canReadWrongUser);
			That(canReadRightUser);
		}
		[Test]
		public async Task CanAddItemsToUser()
		{
			await _userService.CreateUser(userDto);
			var retrievedUser = await _userService.GetUserByName(user.Username);

			var items = new List<Item>() { new Item() { Name = "Some item", Price = 50 }, new Item() { Name = "Some item2", Price = 50 } };

			await _userService.AddItemsToUser(retrievedUser.Id, items);
			var retrievedUserAfter = await _userService.GetUserByName(user.Username);

			That(retrievedUser != null);
			That(retrievedUserAfter.Inventory.Items.Count == 2);
		}

		[Test]
		public async Task CanAddItemToUser()
		{
			await _userService.CreateUser(userDto);
			var retrievedUser = await _userService.GetUserByName(user.Username);

			var item = new Item() { Name = "Some item2", Price = 50 };

			await _userService.AddItemToUser(retrievedUser.Id, item);
			var retrievedUserAfter = await _userService.GetUserByName(user.Username);

			That(retrievedUser != null);
			That(retrievedUserAfter.Inventory.Items.Count == 1);
		}

		[Test]
		public async Task CanUpdateUsersItem()
		{
			await _userService.CreateUser(userDto);
			var retrievedUser = await _userService.GetUserByName(user.Username);

			var item = new Item() { Name = "Some item2", Price = 50 };

			await _userService.AddItemToUser(retrievedUser.Id, item);
			var retrievedUserToUpdate = await _userService.GetUserByName(user.Username);

			var itemToUpdate = retrievedUserToUpdate.Inventory.Items.First();
			itemToUpdate.Name = "Some other item";
			await _userService.UpdateUser(retrievedUserToUpdate);

			var finalUser = await _userService.GetUserByName(user.Username);
			That(finalUser != null);
			That(finalUser.Inventory.Items.Count == 1);
			That(finalUser.Inventory.Items.First().Name == "Some other item");
		}

		[Test]
		public async Task CanGetAll()
		{
			usersToAdd.ForEach(async x => await _userService.CreateUser(x));

			var retrievedUsers = await _userService.GetAllUsers();
			That(retrievedUsers.Count() == usersToAdd.Count());
		}

		[Test]
		public async Task CanGetByID()
		{
			await _userService.CreateUser(userDto);
			Guid id = await Task.FromResult(_userService.GetUserByName(user.Username).Result.Id);
			var retrievedUser = await _userService.GetUserById(id);

			That(retrievedUser != null);
			That(id == retrievedUser.Id);
		}

		[Test]
		public async Task CanDeleteUser()
		{
			await _userService.CreateUser(userDto);

			var retrievedUser = await _userService.GetUserByName(user.Username);

			await _userService.DeleteUser(retrievedUser.Id);

			var retrievedUserAfter = await _userService.GetUserByName(user.Username);
			That(retrievedUser != null);
			That(retrievedUserAfter == null);
		}



	}
}
