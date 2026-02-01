using Microsoft.AspNetCore.Mvc;
using ArticleService.Models.DTOs;
using ArticleService.Services;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            try
            {
                var category = await _service.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            try
            {
                var category = await _service.UpdateCategoryAsync(id, dto);
                if (category == null) return NotFound();
                return Ok(category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteCategoryAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new { error = "Search term is required" });

            var categories = await _service.SearchCategoriesAsync(term);
            return Ok(categories);
        }
    }
}