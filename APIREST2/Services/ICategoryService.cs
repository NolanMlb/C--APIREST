using APIREST2.Models;

namespace APIREST2.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> CreateCategory(Category category);
        Task<bool> DeleteCategory(int id);
    }
}