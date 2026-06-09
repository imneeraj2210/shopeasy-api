using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Product> Products { get; set; }
            = new();
    }
}