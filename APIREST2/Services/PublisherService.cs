using APIREST2.Models;
using APIREST2.Data;
using Microsoft.EntityFrameworkCore;

namespace APIREST2.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(ApplicationDbContext context, ILogger<PublisherService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Publisher>> GetAllPublishers()
        {
            try
            {
                var publishers = await _context.Publishers.ToListAsync();
                _logger.LogInformation("Retrieved {Count} publishers", publishers.Count);
                return publishers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving publishers");
                throw;
            }
        }

        public async Task<Publisher> CreatePublisher(Publisher publisher)
        {
            try
            {
                _logger.LogDebug("Creating a new publisher with name {Name}", publisher.Name);
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new publisher with ID {Id}", publisher.Id);
                return publisher;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating publisher");
                throw new InvalidOperationException("Error creating publisher. The name might already exist.", ex);
            }
        }

        public async Task<bool> DeletePublisher(int id)
        {
            try
            {
                var publisher = await _context.Publishers
                    .Include(p => p.Books)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (publisher == null)
                {
                    _logger.LogWarning("Publisher with ID {Id} not found", id);
                    return false;
                }

                if (publisher.Books.Any())
                {
                    _logger.LogWarning("Cannot delete publisher {Id} - has associated books", id);
                    throw new InvalidOperationException("Cannot delete publisher with associated books");
                }

                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted publisher with ID {Id}", id);
                return true;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error deleting publisher with ID {Id}", id);
                throw;
            }
        }
    }
}