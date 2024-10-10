using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceUnitTests.Fakes
{
	[Table("Items", Schema = "userdata")]
	public class FakeItem
	{
		[Key]
		[Column("ItemId")]
		public Guid Id { get; set; }

		[ForeignKey("InventoryId")]
		public Guid InventoryId { get; set; }

		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public double Price { get; set; }
	}
}
