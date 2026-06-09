namespace ECommerce.API.DTOs
{
    public class OrderCreateDto
    {
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}
