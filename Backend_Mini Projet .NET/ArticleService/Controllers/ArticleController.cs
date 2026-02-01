using ArticleService.Models.DTOs;
using ArticleService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase  // UNE SEULE CLASSE
    {
        private readonly IArticleService _service;

        public ArticleController(IArticleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleCreateDto dto)
        {
            var article = await _service.CreateArticleAsync(dto);
            return CreatedAtAction(nameof(GetArticleById), new { id = article.Id }, article);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)  // Renommé pour éviter conflit
        {
            var article = await _service.GetArticleByIdAsync(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _service.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] ArticleCreateDto dto)
        {
            var article = await _service.UpdateArticleAsync(id, dto);
            if (article == null) return NotFound();
            return Ok(article);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var success = await _service.DeleteArticleAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/warranty-status")]
        public async Task<IActionResult> CheckWarrantyStatus(int id, [FromQuery] DateTime purchaseDate)
        {
            var isUnderWarranty = await _service.IsArticleUnderWarrantyAsync(id, purchaseDate);
            return Ok(new { ArticleId = id, IsUnderWarranty = isUnderWarranty });
        }
    }
}