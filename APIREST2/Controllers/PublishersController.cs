using Microsoft.AspNetCore.Mvc;
using APIREST2.Models;
using APIREST2.Services;

namespace APIREST2.Controllers
{
    [Route("api/publishers")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        /// <summary>
        /// Retrieves all publishers.
        /// </summary>
        /// <returns>A list of publishers.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            var publishers = await _publisherService.GetAllPublishers();
            return Ok(publishers);
        }

        /// <summary>
        /// Creates a new publisher.
        /// </summary>
        /// <param name="publisher">The publisher to create.</param>
        /// <returns>The created publisher.</returns>
        [HttpPost]
        public async Task<ActionResult<Publisher>> CreatePublisher(Publisher publisher)
        {
            try
            {
                var createdPublisher = await _publisherService.CreatePublisher(publisher);
                return CreatedAtAction(nameof(GetPublishers), new { id = createdPublisher.Id }, createdPublisher);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates a publisher.
        /// </summary>
        /// <param name="id">The id of the publisher to update.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            try
            {
                var result = await _publisherService.DeletePublisher(id);
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