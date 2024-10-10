using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceUnitTests.Fakes
{
	internal class FakeContext : DbContext
	{
		public DbSet<FakeUser> Users { get; set; }

		public FakeContext(DbContextOptions<FakeContext> options)
			: base(options)
		{
		}



		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{

		}
	}
}
