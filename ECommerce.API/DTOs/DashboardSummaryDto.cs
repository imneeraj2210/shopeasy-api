namespace ECommerce.API.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReviews { get; set; }
        public int TotalUsers { get; set; }
    }
}
