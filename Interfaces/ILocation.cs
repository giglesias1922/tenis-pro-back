using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface ILocation
    {
        Task<List<Location>> GetAll();
        Task<Location> GetById(string id);
        Task Post(Location location);
        Task<bool> Put(string id, Location location);
        Task<bool> Delete(string id);

    }
}
