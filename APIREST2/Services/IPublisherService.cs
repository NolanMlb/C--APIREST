using APIREST2.Models;

namespace APIREST2.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<Publisher>> GetAllPublishers();
        Task<Publisher> CreatePublisher(Publisher publisher);
        Task<bool> DeletePublisher(int id);
    }
}