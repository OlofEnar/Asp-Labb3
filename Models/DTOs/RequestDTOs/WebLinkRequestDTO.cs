namespace Asp_Labb3.Models.DTOs.RequestDTOs
{
	public class WebLinkRequestDTO
	{
		public int WebLinkId { get; set; }
		public string? Url { get; set; }
		public int InterestId {  get; set; }
		public int UserId { get; set; }
	}
}
