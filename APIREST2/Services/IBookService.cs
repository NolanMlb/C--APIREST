using APIREST2.Models;

namespace APIREST2.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooks(int page, int pageSize, string sortBy, string sortOrder, 
            string? author = null, int? year = null, int? categoryId = null, int? publisherId = null);
        Task<Book?> GetBookById(int id);
        Task<Book> CreateBook(Book book);
        Task<Book?> UpdateBook(int id, Book book);
        Task<bool> DeleteBook(int id);
        Task<int> GetTotalCount();
    }
}