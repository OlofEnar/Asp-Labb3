using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asp_Labb3.Models
{
	public class UserInterest
	{
		[Key]
		public int UserInterestId { get; set; }

		[ForeignKey("User")]
		public int FkUserId { get; set; }
		public virtual User? User { get; set; }

		
		[ForeignKey("Interest")]
		public int FkInterestId { get; set; }
		public virtual Interest? Interest { get; set; }

	}
}
