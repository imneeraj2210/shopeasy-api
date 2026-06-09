using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class ProfileUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }
    }
}
