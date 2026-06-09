using ECommerce.API.Data;
using ECommerce.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var summary = new DashboardSummaryDto
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalUsers = await _context.AppUsers.CountAsync()
            };

            return Ok(summary);
        }

        [HttpGet("charts")]
        public async Task<IActionResult> GetDashboardCharts()
        {
            var currentYear = DateTime.UtcNow.Year;

            var revenueByMonth = await _context.Orders
                .Where(o => o.OrderDate.Year == currentYear)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    MonthNumber = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.MonthNumber)
                .ToListAsync();

            var revenueResponse = revenueByMonth
                .Select(x => new RevenueByMonthDto
                {
                    Month = new DateTime(currentYear, x.MonthNumber, 1).ToString("MMM"),
                    Revenue = x.Revenue
                })
                .ToList();

            var statusOrder = new[]
            {
                "Placed",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            var statusCounts = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new OrdersByStatusDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var ordersByStatus = statusOrder
                .Select(status => new OrdersByStatusDto
                {
                    Status = status,
                    Count = statusCounts.FirstOrDefault(x => x.Status == status)?.Count ?? 0
                })
                .ToList();

            var productsByCategory = await _context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category != null ? p.Category.Name : "Uncategorized")
                .Select(g => new ProductsByCategoryDto
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 10)
                .OrderBy(p => p.Stock)
                .Select(p => new LowStockProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stock = p.Stock
                })
                .ToListAsync();

            return Ok(new
            {
                revenueByMonth = revenueResponse,
                ordersByStatus,
                productsByCategory,
                lowStockProducts
            });
        }
    }
}
