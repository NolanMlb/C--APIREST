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

        // GET: api/loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans([FromQuery] int? bookId, [FromQuery] string? borrowerName, [FromQuery] DateTime? borrowDate)
        {
            var loans = await _loanService.GetAllLoans(bookId, borrowerName, borrowDate);
            return Ok(loans);
        }

        // POST: api/loans
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

        // PUT: api/loans/{id}/return
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