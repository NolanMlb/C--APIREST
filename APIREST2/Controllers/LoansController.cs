using Microsoft.AspNetCore.Mvc;
using APIREST2.Models;
using APIREST2.Services;

namespace APIREST2.Controllers
{
    [Route("api/loans")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Retrieves all loans.
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="borrowerName">The name of the borrower.</param>
        /// <param name="borrowDate">The date the book was borrowed.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans([FromQuery] int? bookId, [FromQuery] string? borrowerName, [FromQuery] DateTime? borrowDate)
        {
            var loans = await _loanService.GetAllLoans(bookId, borrowerName, borrowDate);
            return Ok(loans);
        }

        /// <summary>
        /// Creates a new loan.
        /// </summary>
        /// <param name="loan"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Loan>> CreateLoan(Loan loan)
        {
            try
            {
                var createdLoan = await _loanService.CreateLoan(loan);
                return CreatedAtAction(nameof(GetLoans), new { id = createdLoan.Id }, createdLoan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns a loan.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/return")]
        public async Task<ActionResult<Loan>> ReturnLoan(int id)
        {
            try
            {
                var updatedLoan = await _loanService.ReturnLoan(id);
                if (updatedLoan == null)
                    return NotFound();

                return Ok(updatedLoan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}