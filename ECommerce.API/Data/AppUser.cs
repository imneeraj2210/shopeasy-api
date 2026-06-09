using ECommerce.API.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Data
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public string? Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
    }
}
