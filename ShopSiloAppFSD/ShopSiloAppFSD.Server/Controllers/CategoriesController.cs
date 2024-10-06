using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // POST: api/Categories
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { message = "Category cannot be null." });
            }

            try
            {
                // Manually map CategoryDto to Category entity
                var category = new Category
                {
                    CategoryName = categoryDto.CategoryName,
                    ParentCategoryId = categoryDto.ParentCategoryId
                };

                await _categoryRepository.AddCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryID }, categoryDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Categories
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { message = "Category cannot be null." });
            }

            try
            {
                // Manually map CategoryDto to Category entity
                var category = new Category
                {
                    CategoryID = categoryDto.CategoryID,
                    CategoryName = categoryDto.CategoryName,
                    ParentCategoryId = categoryDto.ParentCategoryId
                };

                await _categoryRepository.UpdateCategoryAsync(category);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/Categories/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryRepository.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Categories/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("Subcategories/{parentCategoryId}")]
        public async Task<IActionResult> GetSubcategories(int parentCategoryId)
        {
            var subcategories = await _categoryRepository.GetSubcategoriesAsync(parentCategoryId);
            return Ok(subcategories);
        }

        [AllowAnonymous]
        [HttpGet("names")]
        public async Task<IActionResult> GetAllCategoryNames()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();

                // Group categories by ParentCategoryId
                var categoryLookup = categories.ToLookup(c => c.ParentCategoryId);

                // Create the hierarchy
                var categoryHierarchy = new List<CategoryNameDto>();

                foreach (var category in categories.Where(c => c.ParentCategoryId == null))
                {
                    var categoryDto = new CategoryNameDto
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName,
                        Icon = category.Icon,
                        SubCategories = categoryLookup[category.CategoryID].Select(c => new CategoryNameDto
                        {
                            CategoryID = c.CategoryID,
                            CategoryName = c.CategoryName,
                            Icon = c.Icon
                        }).ToList()
                    };

                    categoryHierarchy.Add(categoryDto);
                }

                return Ok(categoryHierarchy);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
