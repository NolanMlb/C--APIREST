using APIREST2.Models;

namespace APIREST2.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> CreateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}