using Microsoft.AspNetCore.Hosting;
using ECommerce.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using ECommerce.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Models;
using AutoMapper;


namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        public ProductController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
        }

        // GET: api/product for customers to see only active and in-stock products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var response =
                _mapper.Map<List<ProductResponseDto>>(products);

            return Ok(response);
        }

        // GET: api/product/all for Admin to see all products including those that are not active or out of stock

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var response =
                _mapper.Map<List<ProductResponseDto>>(
                    products);

            return Ok(response);
        }
        // GET: api/product/category/3
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            var response = _mapper.Map<List<ProductResponseDto>>(products);

            return Ok(response);
        }

        // GET: api/product/search/laptop
        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p =>
                    p.Name.ToLower().Contains(keyword.ToLower()) ||
                    p.Description.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();

            var response = _mapper.Map<List<ProductResponseDto>>(products);

            return Ok(response);
        }

        // GET: api/product/sort/price-asc
        [HttpGet("sort/{sortBy}")]
        public async Task<IActionResult> SortProducts(string sortBy)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            switch (sortBy.ToLower())
            {
                case "price-asc":
                    query = query.OrderBy(p => p.Price);
                    break;

                case "price-desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;

                default:
                    return BadRequest(
                        "Invalid sort option.");
            }

            var products = await query.ToListAsync();

            var response =
                _mapper.Map<List<ProductResponseDto>>(
                    products);

            return Ok(response);
        }

        // GET: api/product/page/2/size/5
        [HttpGet("page/{pageNumber}/size/{pageSize}")]
        public async Task<IActionResult> GetPagedProducts(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 8;
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .AsQueryable();

            var totalItems = await query.CountAsync();

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = _mapper.Map<List<ProductResponseDto>>(products);

            return Ok(new
            {
                items = response,
                pageNumber,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }
        // GET: api/product/filter?categoryId=3&keyword=laptop&sortBy=price-asc&pageNumber=1&pageSize=5
        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(int? categoryId, string? keyword, string? sortBy, int pageNumber = 1, int pageSize = 5)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Category Filter
            if (categoryId.HasValue)
            {
                query = query.Where(
                    p => p.CategoryId == categoryId);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword));
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price-asc":
                        query = query.OrderBy(p => p.Price);
                        break;

                    case "price-desc":
                        query = query.OrderByDescending(
                            p => p.Price);
                        break;
                }
            }

            // Pagination
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var products = await query.ToListAsync();

            var response =
                _mapper.Map<List<ProductResponseDto>>(
                    products);

            return Ok(response);
        }

        // POST: api/product
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            var response = _mapper.Map<ProductResponseDto>(product);

            return Ok(response);    
        }
        // POST: api/product/upload-image
        [Authorize(Roles = "Admin")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileName =
                Guid.NewGuid().ToString()
                + Path.GetExtension(file.FileName);

            var folderPath =
                Path.Combine(
                    _environment.WebRootPath,
                    "images");

            var filePath =
                Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl =
                $"/images/{fileName}";

            return Ok(new { imageUrl });
        }
        // GET: api/product/5   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var response =
                _mapper.Map<ProductResponseDto>(product);

            return Ok(response);
        }

        // GET: api/product/5/related
        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (!product.CategoryId.HasValue)
            {
                return Ok(new List<ProductResponseDto>());
            }

            var relatedProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Id != id && p.CategoryId == product.CategoryId)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            var response = _mapper.Map<List<ProductResponseDto>>(relatedProducts);

            return Ok(response);
        }

        // PUT: api/product/id
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            
            if (product == null) { return NotFound("Product not found!"); }

            _mapper.Map(dto, product);

            await _context.SaveChangesAsync();

            var response = _mapper.Map<ProductResponseDto>(product);

            return Ok(response);
        }
        // DELETE: api/product/id
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) { return NotFound("Product not found!"); }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Product deleted successfully!");
        }
    }
}
