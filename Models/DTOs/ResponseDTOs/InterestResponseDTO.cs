namespace Asp_Labb3.Models.DTOs.ResponseDTOs
{
	public class InterestResponseDTO
	{
		public int InterestId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public virtual ICollection<WebLinkResponseDTO>? WebLinks { get; set; }
	}
}
