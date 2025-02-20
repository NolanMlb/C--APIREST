using Microsoft.AspNetCore.Mvc;
using APIREST2.Models;
using APIREST2.Services;

namespace APIREST2.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Retrieves all books.
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves all books. It supports filtering by author, year, category, and publisher.
        /// Also, it supports pagination and sorting.
        /// </remarks>
        /// <param name="author">The author of the book.</param>
        /// <param name="year">The year of publication.</param>
        /// <param name="categoryId"></param>
        /// <param name="publisherId"></param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(
            [FromQuery] string? author,
            [FromQuery] int? year,
            [FromQuery] int? categoryId,
            [FromQuery] int? publisherId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "title",
            [FromQuery] string sortOrder = "asc")
        {
            var books = await _bookService.GetAllBooks(page, pageSize, sortBy, sortOrder, 
                author, year, categoryId, publisherId);

            var totalCount = await _bookService.GetTotalCount();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Total-Pages", totalPages.ToString());

            return Ok(books);
        }

        /// <summary>
        /// Retrieves a specific book.
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves a specific book by its ID.
        /// </remarks>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBookById(id);

            if (book == null)
                return NotFound();

            return book;
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <remarks>
        /// This endpoint creates a new book. It requires a title, author, year, isbn, category, and publisher.
        /// </remarks>
        /// <param name="book">The book to create.</param>
        /// <returns>The created book.</returns>
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            try
            {
                var createdBook = await _bookService.CreateBook(book);
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates a book.
        /// </summary>
        /// <param name="id">The id of the book to update.</param>
        /// <param name="book">The updated book.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            try
            {
                var updatedBook = await _bookService.UpdateBook(id, book);
                if (updatedBook == null)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a book.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var result = await _bookService.DeleteBook(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}