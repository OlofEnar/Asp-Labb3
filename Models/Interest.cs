using System.ComponentModel.DataAnnotations;

namespace Asp_Labb3.Models
{
	public class Interest
	{
		[Key]
		public int InterestId { get; set; }
		
		[Required]
		public string Title { get; set; }
		public string? Description { get; set; }

		public virtual ICollection<UserInterest>? UserInterests { get; set; }
		public virtual ICollection<WebLink>? WebLinks { get; set; }
	}
}
