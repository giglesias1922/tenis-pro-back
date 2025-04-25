using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IFunctionality
    {
        Task<List<Functionality>> GetAll();
        Task<Functionality> GetById(string id);
        Task Post(Functionality Functionality);
        Task Put(string id, Functionality Functionality);
        Task Delete(string id);

    }
}
