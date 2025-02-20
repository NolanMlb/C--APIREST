using APIREST2.Models;
using APIREST2.Data;
using Microsoft.EntityFrameworkCore;

namespace APIREST2.Services
{
    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ApplicationDbContext context, ILogger<LoanService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Loan>> GetAllLoans(int? bookId = null, string? borrowerName = null, DateTime? borrowDate = null)
        {
            try
            {
                var query = _context.Loans
                    .Include(l => l.Book)
                        .ThenInclude(b => b.Category)
                    .Include(l => l.Book)
                        .ThenInclude(b => b.Publisher)
                    .AsQueryable();
        
                if (bookId.HasValue)
                {
                    query = query.Where(l => l.BookId == bookId.Value);
                }
        
                if (!string.IsNullOrEmpty(borrowerName))
                {
                    query = query.Where(l => l.BorrowerName.Contains(borrowerName));
                }
        
                if (borrowDate.HasValue)
                {
                    query = query.Where(l => l.BorrowDate.Date == borrowDate.Value.Date);
                }
        
                var loans = await query.ToListAsync();
                _logger.LogInformation("Retrieved {Count} loans", loans.Count);
                return loans;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving loans");
                throw;
            }
        }

        public async Task<Loan> CreateLoan(Loan loan)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(c => c.Title == loan.Book.Title);
                if (book != null)
                {
                    loan.Book = book;
                }
                else
                {
                    _context.Books.Add(loan.Book);
                }
                _context.Loans.Add(loan);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new loan with ID {Id}", loan.Id);
                return loan;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating loan");
                throw new InvalidOperationException("Error creating loan.", ex);
            }
        }

        public async Task<Loan> ReturnLoan(int id)
        {
            try
            {
                var loan = await _context.Loans.FindAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning("Loan with ID {Id} not found", id);
                    return null;
                }

                loan.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated return date for loan with ID {Id}", id);
                return loan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating return date for loan with ID {Id}", id);
                throw;
            }
        }
    }
}