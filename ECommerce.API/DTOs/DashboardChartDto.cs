namespace ECommerce.API.DTOs
{
    public class RevenueByMonthDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class OrdersByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ProductsByCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class LowStockProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}
