using AutoMapper;
using ECommerce.API.Data;
using Microsoft.AspNetCore.Mvc;
using ECommerce.API.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ECommerce.API.Models;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            var response = _mapper.Map<List<CategoryResponseDto>>(categories);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var response =
                _mapper.Map<CategoryResponseDto>(category);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto dto)
        { 
            var category = _mapper.Map<Category>(dto);

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            var response = _mapper.Map<CategoryResponseDto>(category);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            _mapper.Map(dto, category);

            await _context.SaveChangesAsync();

            var response =
                _mapper.Map<CategoryResponseDto>(category);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return Ok("Category deleted successfully!");
        }
    }
}
