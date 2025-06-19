using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface IParameter
    {
        Task<IEnumerable<Parameter>> GetAll();
        Task<Parameter> GetById(string id);
        Task<Parameter> Post(Parameter param);
        Task Put(string id, Parameter param);
        Task Delete(string id);
    }
}
