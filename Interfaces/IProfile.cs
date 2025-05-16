using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IProfile
    {
        Task<IEnumerable<Profile>> GetAll();
        Task<Profile> GetById(string id);
        Task<Profile> Post(Profile profile);
        Task Put(string id, Profile profile);
        Task Delete(string id);
        Task<Profile?> GetByType(Profile.ProfileType type);

    }
}
