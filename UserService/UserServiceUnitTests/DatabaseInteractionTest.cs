using EF_InteractionFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static NUnit.Framework.Assert;
using UserServiceUnitTests.Fakes;
using UserServiceUnitTests.Utility;

namespace UserServiceUnitTests
{
	/// <summary>
	/// Tests all the generic methods provided in repository
	/// </summary>
	public class DatabaseInteractionTest
	{
		private ServiceProvider _serviceProvider;
		private FakeUser user;
		private List<FakeUser> users;

		[SetUp]
		public void Setup()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			user = CreateUser();
			users = CreateUsers();

		}


		[TearDownAttribute]
		public async Task Dispose()
		{
			await _serviceProvider.DisposeAsync();
		}

		public static FakeUser CreateUser()
		{
			var user = new FakeUser { Username = "something", Password = "test" };
			user.Inventory.Items.Add(new FakeItem() { Name = "Some item", Price = 50 });
			return user;
		}

		public static List<FakeUser> CreateUsers()
		{

			List<FakeUser> users = new List<FakeUser>()
			{
				new FakeUser { Username = "something", Password = "test" },
				new FakeUser { Username = "something1", Password = "test1" },
				new FakeUser { Username = "something2", Password = "test2" }
			};

			users.ForEach(i => i.Inventory.Items.Add(new FakeItem() { Name = "Some item", Price = 50 }));
			return users;
		}

		[Test]
		public async Task CorrectlyCreatesContext()
		{
			using var scope = _serviceProvider.CreateScope();
			var dbContextFactory = _serviceProvider.GetService<IDbContextFactory<FakeContext>>();
			var context = await dbContextFactory?.CreateDbContextAsync()!;
			That(context.Users != null);
		}

		[Test]
		public async Task CanAdd()
		{

			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
		
			await _repository.AddItem(user);
			var result = await _repository.GetItem<FakeUser>(q => q.Where(x => x.Username == user.Username).Include(x => x.Inventory).Include(c => c.Inventory.Items));
			That(result != null);
			That(result!.Inventory.Items.First().Name == "Some item");
		}

		[Test]
		public async Task CanAddMultiple()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItems(users);

			var result = await _repository.GetAllItems<FakeUser>();
			That(result.Count == 3);

		}

		[Test]
		public async Task CanRemove()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItem(user);
			var addedItem = await _repository.GetItem<FakeUser>(q => q.Where(x => x.Id == user.Id));
			That(addedItem != null);

			await _repository.RemoveItem(user);

			var result = await _repository.GetItem<FakeUser>(q => q.Where(x => x.Id == user.Id));
			That(result == null);

		}


		[Test]
		public async Task CanRemoveByExpression()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItem(user);

			var addedItem = await _repository.GetItem<FakeUser>(q => q.Where(x => x.Id == user.Id));
			That(addedItem != null);

			await _repository.RemoveItem<FakeUser>(x => x.Id == user.Id);

			var result = await _repository.GetAllItems<FakeUser>();
			Is.EqualTo(result.Count == 0);
		}


		[Test]
		public async Task CanRemoveMultiple()
		{
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItems(users);
			var tmp = await _repository.GetAllItems<FakeUser>();
			bool wasAdded = tmp != null;
			await _repository.RemoveItems<FakeUser>(users);

			var result = await _repository.GetAllItems<FakeUser>();
			That(result.Count == 0);
			That(wasAdded);
		}

		[Test]
		public async Task CanRemoveMultipleByExpression()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			users.First().Username = "t";
			await _repository.AddItems(users);
			await _repository.RemoveItems<FakeUser>(x => x.Where(y => y.Username.Length > 2));

			var result = await _repository.GetAllItems<FakeUser>();
			That(result.Count == 1);
			That(result.Where(x => x.Username == "something1").Count() == 0);
		}

		[Test]
		public async Task CanGetByExpression()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await _repository.AddItem(user);

			var result = await _repository.GetItem<FakeUser>(q => q.Where(x => x.Id == user.Id));
			That(result?.Username, Is.EqualTo("something"));

		}


		[Test]
		public async Task CanGetByExpressionWithCollection()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItem(user);

			var result = await _repository.GetItem<FakeUser>(q => q.Where(x => x.Id == user.Id).Include(x => x.Inventory));

			That(result?.Username, Is.EqualTo("something"));
			That(result?.Inventory.Id == user.Inventory.Id);
		}


		[Test]
		public async Task CanGetAllWithoutExpression()
		{
			var otherUser = new FakeUser();
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();


			await _repository.AddItem(user);
			await _repository.AddItem(otherUser);

			var result = await _repository.GetAllItems<FakeUser>();
			That(result.Count == 2);

		}

		[Test]
		public async Task CanGetAllWithExpression()
		{
			var otherUser = new FakeUser();

			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItem(user);
			await _repository.AddItem(otherUser);

			var result = await _repository.GetAllItems<FakeUser>(q => q.Where(x => x.Id == otherUser.Id));
			That(result.Count == 1);


		}

		[Test]
		public async Task CanGetAllForColumn()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider<FakeContext>();
			using var scope = _serviceProvider.CreateScope();
			var _repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			await _repository.AddItems(users);

			List<string> result = await _repository.GetAllForColumn<FakeUser, string>(x => x.Select(s => s.Username)!);
			List<int> test2Result = await _repository.GetAllForColumnStruct<FakeUser, int>(x => x.Select(s => s.StructvalueForTest));
			That(result.Count == users.Count());
		}


		[Test]
		public async Task CanUpdateItem()
		{
			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItem(user);
			var addedUser = repository.GetItem<FakeUser>(x => x.Where(q => q.Username == user.Username)).Result;
			addedUser.StructvalueForTest = 1;
			await repository.UpdateItem(addedUser);

			var result = await repository.GetItem<FakeUser>(x => x.Where(q => q.Id == addedUser.Id).Include(i => i.Inventory.Items));
			That(result.StructvalueForTest == 1);
		}


		[Test]
		public async Task CanUpdateMultipleItems()
		{
			var users = CreateUsers();
			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(users);

			var retrievesUsers = repository.GetAllItems<FakeUser>(q => q.Include(i => i.Inventory.Items)).Result;
			foreach (var item in retrievesUsers)
			{
				item.StructvalueForTest = 4;
			}

			await repository.UpdateItems(retrievesUsers);
			var result = await repository.GetAllItems<FakeUser>(q => q.Include(i => i.Inventory.Items));
			That(result.TrueForAll(x => x.StructvalueForTest == 4));
		}
	}
}
