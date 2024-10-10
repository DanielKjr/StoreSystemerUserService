using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
	[Table("Users", Schema = "userdata")]
	public class User
	{
		[Key]
		[Column("UserId")]
		public Guid Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;

		public Inventory Inventory { get; set; } = new Inventory();


	}
}
