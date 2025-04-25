using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IUser
    {
        Task<IEnumerable<User>> GetAll();
        Task<IEnumerable<User>> GetByCategory(string categoryId);
        Task<User> GetById(string id);
        Task Post(User user);
        Task Put(string id, User user);
        Task Delete(string id);
    }
}
