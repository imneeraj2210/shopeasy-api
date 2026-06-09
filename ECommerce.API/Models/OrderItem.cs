using InventoryAPI.Models;

namespace ECommerce.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int Quantity { set; get; }

        public decimal Price { set; get; }
        public int ProductId { set; get; }
        public Product Product { get; set; } = null!;

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;
    }
}
