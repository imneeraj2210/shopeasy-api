using ECommerce.API.Data;
using ECommerce.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Placed";

        // FK
        public int UserId { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public ICollection<OrderItem>
            OrderItems
        { get; set; }
            = new List<OrderItem>();
    }
}