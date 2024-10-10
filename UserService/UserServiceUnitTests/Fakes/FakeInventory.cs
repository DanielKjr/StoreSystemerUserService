using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceUnitTests.Fakes
{
	[Table("Inventory", Schema = "userdata")]
	public class FakeInventory
	{

		[Key]
		[Column("InventoryId")]
		public Guid Id { get; set; }

		[ForeignKey("UserId")]
		public Guid UserId { get; set; }

		public ICollection<FakeItem> Items { get; set; } = new List<FakeItem>();

	}
}
