using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
	[Table("Items", Schema = "userdata")]
	public class Item
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
