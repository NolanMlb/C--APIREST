using APIREST2.Models;

namespace APIREST2.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetAllLoans(int? bookId = null, string? borrowerName = null, DateTime? borrowDate = null);
        Task<Loan> CreateLoan(Loan loan);
        Task<Loan> ReturnLoan(int id);
    }
}