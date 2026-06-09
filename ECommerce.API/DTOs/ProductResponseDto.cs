    namespace ECommerce.API.DTOs
    {
        public class ProductResponseDto
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public decimal Price { get; set; }

            public int Stock { get; set; }

            public string? ImageUrl { get; set; }

            public int? CategoryId { get; set; }

            public string? CategoryName { get; set; }

            public double AverageRating { get; set; }

            public int ReviewCount { get; set; }
        }
    }
