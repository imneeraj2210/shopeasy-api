using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters and include one uppercase letter, one lowercase letter, one number, and one special character."
        )]
        public string? Password { get; set; }
    }
}
