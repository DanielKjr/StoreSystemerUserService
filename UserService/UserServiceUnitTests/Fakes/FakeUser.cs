using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceUnitTests.Fakes
{
	[Table("Users", Schema = "userdata")]
	public class FakeUser
	{
		[Key]
		[Column("UserId")]
		public Guid Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;

		public int StructvalueForTest { get; set; } = 0;
		public FakeInventory Inventory { get; set; } = new FakeInventory();


	}
}
