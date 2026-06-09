using AutoMapper;
using ECommerce.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.DTOs;
using ECommerce.API.Models;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewCreateDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(
                p => p.Id == dto.ProductId
            );

            if (product == null)
            {
                return BadRequest($"Product with ID {dto.ProductId} not found.");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            var review = _mapper.Map<Review>(dto);

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

            var response = _mapper.Map<ReviewResponseDto>(review);

            return Ok(response);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewByProduct(int productId)
        {
            var review = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAT)
                .ToListAsync();

            var response = _mapper.Map<List<ReviewResponseDto>>(review);

            return Ok(response);
        }

        [HttpGet("product/{productId}/summery")]
        public async Task<IActionResult> GetReviewSummery(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
            {
                return Ok(new ReviewSummaryDto
                {
                    AverageRating = 0,
                    ReviewCount = 0
                } );
            }

            var averageRating = reviews.Average(r => r.Rating);
            var reviewCount = reviews.Count;

            var summary = new ReviewSummaryDto
            {
                AverageRating = Math.Round(reviews.Average(r => r.Rating), 2),
                ReviewCount = reviewCount
            };

            return Ok(summary);
        }
    }
}
