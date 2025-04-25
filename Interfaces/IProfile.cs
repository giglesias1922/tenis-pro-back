using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IProfile
    {
        Task<IEnumerable<Profile>> GetAll();
        Task<Profile> GetById(string id);
        Task Post(Profile profile);
        Task Put(string id, Profile profile);
        Task Delete(string id);

    }
}
