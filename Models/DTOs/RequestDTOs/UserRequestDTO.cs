namespace Asp_Labb3.Models.DTOs.RequestDTOs
{
	public class UserRequestDTO
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string UserEmail { get; set; }
		public ICollection<InterestRequestDTO>? Interests { get; set; }
	}
}
