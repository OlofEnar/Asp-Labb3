using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Asp_Labb3.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		
		[Required]
		public string UserName { get; set; }
		public string? UserEmail { get; set; }

		public virtual ICollection<UserInterest>? UserInterests { get; set; }

	}
}