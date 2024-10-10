using EF_InteractionFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Context;
using UserService.Interfaces;
using UserService.Repository;

namespace UserServiceUnitTests.Utility
{
	/// <summary>
	/// Static class to provide a service provider for testing
	/// </summary>
	public static class TestServiceProvider
	{

		/// <summary>
		/// Returns a ServiceProvider with the necessary services for testing
		/// </summary>
		/// <returns></returns>
		public static ServiceProvider GetServiceProvider<TContext>() where TContext : DbContext
		{
			var services = new ServiceCollection();
			var random = new Random();

			// Generate a unique name for the in-memory database
			var randomName = $"{DateTime.Now:yyyyMMddHHmmss}_{random.Next(1, 500)}";

			// Configure the in-memory database
			services.AddDbContextFactory<TContext>(options =>
				options.UseInMemoryDatabase($"TestDatabase_{randomName}"));

			if (typeof(TContext) == typeof(UserContext))
			{
				// dummy configuration 
				var configuration = new ConfigurationBuilder().AddInMemoryCollection(
					new Dictionary<string, string>
					{
				{ "ConnectionStrings:sqlite", $"Data Source=TestDatabase_{randomName}.db" }
					}!).Build();

				services.AddSingleton<IConfiguration>(configuration);
			}
			services.AddTransientAsyncRepository<TContext>();
			services.AddTransient<IRepositoryPatch<TContext>, RepositoryPatch<TContext>>();

			return services.BuildServiceProvider();
		}

	}
}
