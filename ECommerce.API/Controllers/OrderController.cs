using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Models;
using InventoryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderController(ApplicationDbContext context, IMapper mapper)
        { 
            _context = context;
            _mapper = mapper;
        }

        // POST: api/order - To place a new order

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(OrderCreateDto dto)
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (
                string.IsNullOrEmpty(
                    userIdClaim
                )
            )
            {
                return Unauthorized(
                    "User not authenticated"
                );
            }

            int userId =
                int.Parse(
                    userIdClaim
                );

            decimal totalAmount = 0;

            var orderItems =
                new List<OrderItem>();

            foreach (
                var item
                in dto.Items
            )
            {
                var product =
                    await _context.Products
                        .FindAsync(
                            item.ProductId
                        );

                if (
                    product == null
                )
                {
                    return NotFound(
                        $"Product {item.ProductId} not found"
                    );
                }

                if (
                    product.Stock <
                    item.Quantity
                )
                {
                    return BadRequest(
                        $"Insufficient stock for {product.Name}"
                    );
                }

                product.Stock -=
                    item.Quantity;

                totalAmount +=
                    product.Price *
                    item.Quantity;

                orderItems.Add(
                    new OrderItem
                    {
                        ProductId =
                            product.Id,

                        Quantity =
                            item.Quantity,

                        Price =
                            product.Price
                    }
                );
            }

            var order =
                new Order
                {
                    OrderDate =
                        DateTime.UtcNow,

                    TotalAmount =
                        totalAmount,

                    Status =
                        "Placed",

                    UserId =
                        userId,

                    OrderItems =
                        orderItems
                };

            _context.Orders.Add(
                order
            );

            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    message =
                        "Order placed successfully",
                    orderId =
                        order.Id
                }
            );
        }

        // GET: api/order/all - To get all orders (Admin only)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]

        // GET: api/order - To get orders for the logged-in user
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var response = orders.Select(order =>
                new
                {
                    order.Id,
                    order.OrderDate,
                    order.TotalAmount,
                    order.Status,
                    CustomerName =
                        order.User != null ? order.User.FullName : "Guest"
                });

            return Ok(response);
        }

        // GET: api/order/5 - To get details of a specific order
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if(order == null)
            {
                return NotFound("Order not found!!");
            }

            var responce = _mapper.Map<OrderResponseDto>(order);

            return Ok(responce);
        }

        // POST: api/order/5/cancel - To cancel an order

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound("Order not found!");
                }

                if (order.Status == "Cancelled")
                {
                    return BadRequest("Order is already cancelled!");
                }

                foreach (var item in order.OrderItems)
                {
                    item.Product.Stock += item.Quantity;
                }

                order.Status = "Cancelled";

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        // PUT: api/order/5/status - To update order status (Admin only)

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id,[FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var response = orders.Select(order => new
            {
                order.Id,
                order.OrderDate,
                order.TotalAmount,
                order.Status,

                Items = order.OrderItems.Select(item => new
                {
                    ProductName = item.Product.Name,
                    item.Quantity,
                    item.Price
                })
            });

            return Ok(response);
        }


    }
}
