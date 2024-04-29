using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Asp_Labb3.Models
{
	public class WebLink
	{
		[Key]
		public int WebLinkId { get; set; }

		[Required]
		public string? Url { get; set; }

		[ForeignKey("Interest")]
		public int FkInterestId { get; set; }
		public Interest? Interest { get; set; }

		[ForeignKey("User")]
		public int FkUserId { get; set; }
		public User? User { get; set; }
	}
}
