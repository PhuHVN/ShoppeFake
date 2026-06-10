namespace ShoppeFake.Application.DTOs.FeedbackDtos
{
    public class FeedbackRequest
    {
        public string AccountId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
