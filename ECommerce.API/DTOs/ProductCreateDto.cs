using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class ProductCreateDto
    {
        public int? CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Range(1, 1000000)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }
    }
}
