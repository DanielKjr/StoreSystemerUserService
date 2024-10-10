using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Context
{
	public class UserContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		private string _connectionString;

		public UserContext(IConfiguration configuration)
		{
			_connectionString = configuration["ConnectionStrings:sqlite"]!;
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(_connectionString);
			base.OnConfiguring(optionsBuilder);
		}

	}
}
