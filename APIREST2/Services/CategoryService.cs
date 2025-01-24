using APIREST2.Models;
using APIREST2.Data;
using Microsoft.EntityFrameworkCore;

namespace APIREST2.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                _logger.LogInformation("Retrieved {Count} categories", categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                throw;
            }
        }

        public async Task<Category> CreateCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new category with ID {Id}", category.Id);
                return category;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating category");
                throw new InvalidOperationException("Error creating category. The name might already exist.", ex);
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Books)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found", id);
                    return false;
                }

                if (category.Books.Any())
                {
                    _logger.LogWarning("Cannot delete category {Id} - has associated books", id);
                    throw new InvalidOperationException("Cannot delete category with associated books");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted category with ID {Id}", id);
                return true;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error deleting category with ID {Id}", id);
                throw;
            }
        }
    }
}