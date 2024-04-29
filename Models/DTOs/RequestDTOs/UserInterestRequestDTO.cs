using System.Text.Json.Serialization;

namespace Asp_Labb3.Models.DTOs.RequestDTOs
{
	public class UserInterestRequestDTO
	{
		public int UserId { get; set; }

		public int InterestId { get; set; }
	}
}
