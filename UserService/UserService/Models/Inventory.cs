using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
	[Table("Inventory", Schema = "userdata")]
	public class Inventory
	{

		[Key]
		[Column("InventoryId")]
		public Guid Id { get; set; }

		[ForeignKey("UserId")]
		public Guid UserId { get; set; }

		public List<Item> Items { get; set; } = new List<Item>();

	}
}
