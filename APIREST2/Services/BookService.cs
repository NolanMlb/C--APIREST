using APIREST2.Models;
using APIREST2.Data;
using Microsoft.EntityFrameworkCore;

namespace APIREST2.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookService> _logger;

        public BookService(ApplicationDbContext context, ILogger<BookService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Book>> GetAllBooks(int page, int pageSize, string sortBy, string sortOrder,
            string? author = null, int? year = null, int? categoryId = null, int? publisherId = null)
        {
            try
            {
                var query = _context.Books
                    .Include(b => b.Category)
                    .Include(b => b.Publisher)
                    .AsQueryable();
        
                // Apply filters
                if (!string.IsNullOrEmpty(author))
                    query = query.Where(b => b.Author.Contains(author));
                if (year.HasValue)
                    query = query.Where(b => b.PublishedYear == year);
                if (categoryId.HasValue)
                    query = query.Where(b => b.CategoryId == categoryId);
                if (publisherId.HasValue)
                    query = query.Where(b => b.PublisherId == publisherId);
        
                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "title" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                    "author" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author),
                    "publishedyear" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.PublishedYear) : query.OrderBy(b => b.PublishedYear),
                    "isbn" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.ISBN) : query.OrderBy(b => b.ISBN),
                    _ => query.OrderBy(b => b.Title) // default sorting
                };
        
                // Apply pagination
                var books = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        
                _logger.LogInformation("Retrieved {Count} books", books.Count);
                return books;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                throw;
            }
        }

        public async Task<Book?> GetBookById(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Category)
                    .Include(b => b.Publisher)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                    _logger.LogWarning("Book with ID {Id} not found", id);
                else
                    _logger.LogInformation("Retrieved book with ID {Id}", id);

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID {Id}", id);
                throw;
            }
        }

        public async Task<Book> CreateBook(Book book)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(c => c.Name == book.Category.Name);
                if (category != null)
                {
                    book.Category = category;
                }
                else
                {
                    _context.Categories.Add(book.Category);
                }

                var publisher = _context.Publishers.FirstOrDefault(p => p.Name == book.Publisher.Name);
                if (publisher != null)
                {
                    book.Publisher = publisher;
                }
                else
                {
                    _context.Publishers.Add(book.Publisher);
                }

                foreach (var loan in book.Loans)
                {
                    var existingLoan = _context.Loans.FirstOrDefault(l => l.BorrowerName == loan.BorrowerName);
                    if (existingLoan != null)
                    {
                        loan.Id = existingLoan.Id;
                    }
                    else
                    {
                        loan.Book = book;
                        _context.Loans.Add(loan);
                    }
                }
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created new book with ID {Id}", book.Id);
                return book;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating book");
                throw new InvalidOperationException("Error creating book. Please check the data and try again.", ex);
            }
        }

        public async Task<Book?> UpdateBook(int id, Book book)
        {
            try
            {
                if (id != book.Id)
                    return null;

                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated book with ID {Id}", id);
                return book;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await BookExists(id))
                {
                    _logger.LogWarning("Book with ID {Id} not found during update", id);
                    return null;
                }
                _logger.LogError(ex, "Concurrency error updating book with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Loans)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {Id} not found during delete", id);
                    return false;
                }

                if (book.Loans?.Any(l => l.ReturnDate == null) == true)
                {
                    _logger.LogWarning("Cannot delete book with ID {Id} - has active loans", id);
                    throw new InvalidOperationException("Cannot delete book with active loans");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted book with ID {Id}", id);
                return true;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error deleting book with ID {Id}", id);
                throw;
            }
        }

        public async Task<int> GetTotalCount()
        {
            try
            {
                return await _context.Books.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total book count");
                throw;
            }
        }

        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }
    }
}