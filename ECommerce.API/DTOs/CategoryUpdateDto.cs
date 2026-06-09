using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class CategoryUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
    }
}
