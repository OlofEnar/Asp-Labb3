namespace Asp_Labb3.Models.DTOs.RequestDTOs
{
	public class InterestRequestDTO
	{
		public int InterestId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public virtual ICollection<WebLinkRequestDTO>? WebLinks { get; set; }
	}
}
