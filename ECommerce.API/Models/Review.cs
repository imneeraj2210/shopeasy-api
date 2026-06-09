namespace ECommerce.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAT { get; set; } = DateTime.UtcNow;

    }
}
