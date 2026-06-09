namespace ECommerce.API.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? CustomerName { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
